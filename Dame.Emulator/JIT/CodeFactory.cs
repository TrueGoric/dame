using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Dame.Emulator.Memory;
using Dame.Emulator.Processor;

[assembly: InternalsVisibleTo("Dame.Tests")]

namespace Dame.Emulator.JIT
{
    internal static partial class CodeFactory
    {
        private static MethodInfo GetRegistersMethod = typeof(ProcessorExecutionContext)
            .GetProperty(nameof(ProcessorExecutionContext.Registers))
            .GetGetMethod();
        private static MethodInfo GetMemoryControllerMethod = typeof(ProcessorExecutionContext)
            .GetProperty(nameof(ProcessorExecutionContext.Memory))
            .GetGetMethod();
        private static MethodInfo CycleMethod = typeof(ProcessorExecutionContext).GetMethod(nameof(ProcessorExecutionContext.Cycle));

        public static ILGenerator EmitNop(this ILGenerator gen)
        {
            gen.Emit(OpCodes.Nop); // temp, will be omitted

            return gen;
        }

        #region Common

        public static DynamicMethod CreateInstructionMethod(string name)
        {
            var method = new DynamicMethod(name, null, new[] { typeof(ProcessorExecutionContext) });
            var gen = method.GetILGenerator();

            InitMethod(gen);

            return method;
        }

        public static DynamicMethod CreateJITMethod(string name)
        {
            var method = new DynamicMethod(name, null, new[] { typeof(ProcessorExecutionContext), typeof(int) });
            var gen = method.GetILGenerator();

            InitMethod(gen);

            return method;
        }

        private static void InitMethod(ILGenerator gen)
        {
            gen.DeclareLocal(typeof(RegisterBank));
            gen.DeclareLocal(typeof(MemoryController));
            gen.DeclareLocal(typeof(byte)); // working var
            gen.DeclareLocal(typeof(ushort)); // working var
            gen.DeclareLocal(typeof(int)); // working var, for addresses

            // load register bank to a local variable
            gen.Emit(OpCodes.Ldarg_0);
            gen.EmitCall(OpCodes.Callvirt, GetRegistersMethod, null);
            gen.Emit(OpCodes.Stloc_0);

            // load memory controller to a local variable
            gen.Emit(OpCodes.Ldarg_0);
            gen.EmitCall(OpCodes.Callvirt, GetMemoryControllerMethod, null);
            gen.Emit(OpCodes.Stloc_1);
        }

        public static void FinishMethod(this ILGenerator gen)
        {
            gen.Emit(OpCodes.Ret);
        }

        #endregion

        #region Control

        public static ILGenerator EmitControlRoutine(this ILGenerator gen, byte cycles = 1)
        {
            EmitCycleCall(gen, cycles);
            EmitInterruptHandler(gen);
            // TODO: check if PC checks out
            // TODO: advance PC

            return gen;
        }

        public static ILGenerator EmitCycleCall(this ILGenerator gen, byte cycles = 1)
        {
            gen.Emit(OpCodes.Ldarg_0); // assumes the ProcessorExecutionContext is the first argument passed to the dynamic method
            gen.Emit(OpCodes.Ldc_I4_S, cycles);
            gen.EmitCall(OpCodes.Call, CycleMethod, null);

            return gen;
        }

        public static ILGenerator EmitInterruptHandler(this ILGenerator gen)
        {
            // TODO: emit interrupts

            return gen;
        }

        #endregion
    }
}
