using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Dame.Emulator.Memory;
using Dame.Emulator.Processor;

[assembly: InternalsVisibleTo("Dame.Tests")]

namespace Dame.Emulator.Instructions
{
    internal static partial class InstructionFactory
    {
        private static MethodInfo GetRegistersMethod = typeof(ProcessorExecutionContext)
            .GetProperty(nameof(ProcessorExecutionContext.Registers))
            .GetGetMethod();
        private static MethodInfo GetMemoryControllerMethod = typeof(ProcessorExecutionContext)
            .GetProperty(nameof(ProcessorExecutionContext.Memory))
            .GetGetMethod();
        private static MethodInfo CycleMethod = typeof(ProcessorExecutionContext).GetMethod(nameof(ProcessorExecutionContext.Cycle));

        public static void EmitNop(ILGenerator gen)
        {
            gen.Emit(OpCodes.Nop); // temp, will be omitted
        }

        #region Common

        public static DynamicMethod CreateJITMethod(string name)
        {
            var method = new DynamicMethod(name, null, new[] { typeof(ProcessorExecutionContext) });
            var gen = method.GetILGenerator();

            gen.DeclareLocal(typeof(RegisterBank));
            gen.DeclareLocal(typeof(MemoryController));
            gen.DeclareLocal(typeof(byte)); // working var
            gen.DeclareLocal(typeof(ushort)); // working var

            // load register bank to a local variable
            gen.Emit(OpCodes.Ldarg_0);
            gen.EmitCall(OpCodes.Callvirt, GetRegistersMethod, null);
            gen.Emit(OpCodes.Stloc_0);

            // load memory controller to a local variable
            gen.Emit(OpCodes.Ldarg_0);
            gen.EmitCall(OpCodes.Callvirt, GetMemoryControllerMethod, null);
            gen.Emit(OpCodes.Stloc_1);

            return method;
        }

        public static void FinishMethod(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ret);
        }

        #endregion

        #region Control

        public static void EmitControlRoutine(ILGenerator gen)
        {
            EmitCycleCall(gen);
            EmitInterruptHandler(gen);
            // TODO: check if PC checks out
            // TODO: advance PC
        }

        public static void EmitCycleCall(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldarg_0); // assumes the ProcessorExecutionContext is the first argument passed to the dynamic method
            gen.EmitCall(OpCodes.Call, CycleMethod, null);
        }

        public static void EmitInterruptHandler(ILGenerator gen)
        {
            // TODO: emit interrupts
        }

        #endregion
    }
}
