using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Dame.Emulator.Memory;
using Dame.Emulator.Processor;

namespace Dame.Emulator.Instructions
{
    public static partial class InstructionFactory
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

        #region Load

        public static void EmitLoad(ILGenerator gen, Register from, Register to)
        {
            LoadRegisterValue(gen, from);
            WriteToRegister(gen, to);
        }

        #endregion

        #region Common

        public static DynamicMethod CreateJITMethod(string name)
        {
            var method = new DynamicMethod(name, null, new[] { typeof(ProcessorExecutionContext) });
            var gen = method.GetILGenerator();

            gen.DeclareLocal(typeof(RegisterBank));
            gen.DeclareLocal(typeof(MemoryController));
            gen.DeclareLocal(typeof(byte)); // working var
            gen.DeclareLocal(typeof(ushort)); //working var

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

        public static void EmitReturn(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ret);
        }

        #region Registers

        #region Method reflection

        private static MethodInfo GetRegisterA = typeof(RegisterBank).GetProperty(nameof(RegisterBank.A)).GetGetMethod();
        private static MethodInfo GetRegisterF = typeof(RegisterBank).GetProperty(nameof(RegisterBank.F)).GetGetMethod();
        private static MethodInfo GetRegisterB = typeof(RegisterBank).GetProperty(nameof(RegisterBank.B)).GetGetMethod();
        private static MethodInfo GetRegisterC = typeof(RegisterBank).GetProperty(nameof(RegisterBank.C)).GetGetMethod();
        private static MethodInfo GetRegisterD = typeof(RegisterBank).GetProperty(nameof(RegisterBank.D)).GetGetMethod();
        private static MethodInfo GetRegisterE = typeof(RegisterBank).GetProperty(nameof(RegisterBank.E)).GetGetMethod();
        private static MethodInfo GetRegisterH = typeof(RegisterBank).GetProperty(nameof(RegisterBank.H)).GetGetMethod();
        private static MethodInfo GetRegisterL = typeof(RegisterBank).GetProperty(nameof(RegisterBank.L)).GetGetMethod();
        private static MethodInfo GetRegisterAF = typeof(RegisterBank).GetProperty(nameof(RegisterBank.AF)).GetGetMethod();
        private static MethodInfo GetRegisterBC = typeof(RegisterBank).GetProperty(nameof(RegisterBank.BC)).GetGetMethod();
        private static MethodInfo GetRegisterDE = typeof(RegisterBank).GetProperty(nameof(RegisterBank.DE)).GetGetMethod();
        private static MethodInfo GetRegisterHL = typeof(RegisterBank).GetProperty(nameof(RegisterBank.HL)).GetGetMethod();
        private static MethodInfo GetRegisterSP = typeof(RegisterBank).GetProperty(nameof(RegisterBank.SP)).GetGetMethod();
        private static MethodInfo GetRegisterPC = typeof(RegisterBank).GetProperty(nameof(RegisterBank.PC)).GetGetMethod();

        private static MethodInfo SetRegisterA = typeof(RegisterBank).GetProperty(nameof(RegisterBank.A)).GetSetMethod();
        private static MethodInfo SetRegisterF = typeof(RegisterBank).GetProperty(nameof(RegisterBank.F)).GetSetMethod();
        private static MethodInfo SetRegisterB = typeof(RegisterBank).GetProperty(nameof(RegisterBank.B)).GetSetMethod();
        private static MethodInfo SetRegisterC = typeof(RegisterBank).GetProperty(nameof(RegisterBank.C)).GetSetMethod();
        private static MethodInfo SetRegisterD = typeof(RegisterBank).GetProperty(nameof(RegisterBank.D)).GetSetMethod();
        private static MethodInfo SetRegisterE = typeof(RegisterBank).GetProperty(nameof(RegisterBank.E)).GetSetMethod();
        private static MethodInfo SetRegisterH = typeof(RegisterBank).GetProperty(nameof(RegisterBank.H)).GetSetMethod();
        private static MethodInfo SetRegisterL = typeof(RegisterBank).GetProperty(nameof(RegisterBank.L)).GetSetMethod();
        private static MethodInfo SetRegisterAF = typeof(RegisterBank).GetProperty(nameof(RegisterBank.AF)).GetSetMethod();
        private static MethodInfo SetRegisterBC = typeof(RegisterBank).GetProperty(nameof(RegisterBank.BC)).GetSetMethod();
        private static MethodInfo SetRegisterDE = typeof(RegisterBank).GetProperty(nameof(RegisterBank.DE)).GetSetMethod();
        private static MethodInfo SetRegisterHL = typeof(RegisterBank).GetProperty(nameof(RegisterBank.HL)).GetSetMethod();
        private static MethodInfo SetRegisterSP = typeof(RegisterBank).GetProperty(nameof(RegisterBank.SP)).GetSetMethod();
        private static MethodInfo SetRegisterPC = typeof(RegisterBank).GetProperty(nameof(RegisterBank.PC)).GetSetMethod();

        #endregion

        public static void LoadRegisterValue(ILGenerator gen, Register register)
        {
            gen.Emit(OpCodes.Ldloc_0);

            switch (register)
            {
                case Register.A: gen.EmitCall(OpCodes.Call, GetRegisterA, null); break;
                case Register.F: gen.EmitCall(OpCodes.Call, GetRegisterF, null); break;
                case Register.B: gen.EmitCall(OpCodes.Call, GetRegisterB, null); break;
                case Register.C: gen.EmitCall(OpCodes.Call, GetRegisterC, null); break;
                case Register.D: gen.EmitCall(OpCodes.Call, GetRegisterD, null); break;
                case Register.E: gen.EmitCall(OpCodes.Call, GetRegisterE, null); break;
                case Register.H: gen.EmitCall(OpCodes.Call, GetRegisterH, null); break;
                case Register.L: gen.EmitCall(OpCodes.Call, GetRegisterL, null); break;

                case Register.AF: gen.EmitCall(OpCodes.Call, GetRegisterAF, null); break;
                case Register.BC: gen.EmitCall(OpCodes.Call, GetRegisterBC, null); break;
                case Register.DE: gen.EmitCall(OpCodes.Call, GetRegisterDE, null); break;
                case Register.HL: gen.EmitCall(OpCodes.Call, GetRegisterHL, null); break;
                case Register.SP: gen.EmitCall(OpCodes.Call, GetRegisterSP, null); break;
                case Register.PC: gen.EmitCall(OpCodes.Call, GetRegisterPC, null); break;

                default: throw new ArgumentException($"{register} is not a valid member of {nameof(Register)} enum!", nameof(register));
            }
        }

        public static void WriteToRegister(ILGenerator gen, Register register)
        {
            // load the last stack value to an appropriate local var and then back to the stack
            switch (register)
            {
                // byte
                case Register.A: case Register.F: case Register.B: case Register.C:
                case Register.D: case Register.E: case Register.H: case Register.L:
                    gen.Emit(OpCodes.Stloc_2);
                    gen.Emit(OpCodes.Ldloc_0);
                    gen.Emit(OpCodes.Ldloc_2);
                    break;
                
                // ushort
                case Register.AF: case Register.BC: case Register.DE: case Register.HL:
                case Register.SP: case Register.PC:
                    gen.Emit(OpCodes.Stloc_3);
                    gen.Emit(OpCodes.Ldloc_0);
                    gen.Emit(OpCodes.Ldloc_3);
                    break;
            }

            switch (register)
            {
                case Register.A: gen.EmitCall(OpCodes.Call, SetRegisterA, null); break;
                case Register.F: gen.EmitCall(OpCodes.Call, SetRegisterF, null); break;
                case Register.B: gen.EmitCall(OpCodes.Call, SetRegisterB, null); break;
                case Register.C: gen.EmitCall(OpCodes.Call, SetRegisterC, null); break;
                case Register.D: gen.EmitCall(OpCodes.Call, SetRegisterD, null); break;
                case Register.E: gen.EmitCall(OpCodes.Call, SetRegisterE, null); break;
                case Register.H: gen.EmitCall(OpCodes.Call, SetRegisterH, null); break;
                case Register.L: gen.EmitCall(OpCodes.Call, SetRegisterL, null); break;

                case Register.AF: gen.EmitCall(OpCodes.Call, SetRegisterAF, null); break;
                case Register.BC: gen.EmitCall(OpCodes.Call, SetRegisterBC, null); break;
                case Register.DE: gen.EmitCall(OpCodes.Call, SetRegisterDE, null); break;
                case Register.HL: gen.EmitCall(OpCodes.Call, SetRegisterHL, null); break;
                case Register.SP: gen.EmitCall(OpCodes.Call, SetRegisterSP, null); break;
                case Register.PC: gen.EmitCall(OpCodes.Call, SetRegisterPC, null); break;

                default: throw new ArgumentException($"{register} is not a valid member of {nameof(Register)} enum!", nameof(register));
            }
        }

        #endregion

        #endregion

        #region Control

        public static void EmitControlRoutine(ILGenerator gen)
        {
            EmitCycleCall(gen);
            EmitInterruptHandler(gen);
            // TODO: check if PC checks out
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
