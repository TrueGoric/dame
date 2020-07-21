using System;
using System.Reflection;
using System.Reflection.Emit;
using Dame.Emulator.Memory;
using Dame.Emulator.Processor;

namespace Dame.Emulator.JIT
{
    internal static partial class CodeFactory
    {
        #region Read & Advance Methods

        private static MethodInfo ReadAndAdvanceMethod = typeof(ProcessorExecutionContext).GetMethod(nameof(ProcessorExecutionContext.ReadAndAdvance));
        private static MethodInfo ReadDoubleAndAdvanceMethod = typeof(ProcessorExecutionContext).GetMethod(nameof(ProcessorExecutionContext.ReadDoubleAndAdvance));

        #endregion
        #region Register Getters/Setters

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
        #region Memory Controller Methods

        private static MethodInfo ReadMemoryMethod = typeof(MemoryController).GetMethod(nameof(MemoryController.Read));
        private static MethodInfo ReadDoubleMemoryMethod = typeof(MemoryController).GetMethod(nameof(MemoryController.ReadDouble));

        private static MethodInfo WriteMemoryMethod = typeof(MemoryController).GetMethod(nameof(MemoryController.Write));
        private static MethodInfo WriteDoubleMemoryMethod = typeof(MemoryController).GetMethod(nameof(MemoryController.WriteDouble));

        #endregion

        public static ILGenerator LoadConstantToStack(this ILGenerator gen, byte value)
        {
            gen.Emit(OpCodes.Ldc_I4_S, value);  // load constant (value)

            return gen;
        }

        public static ILGenerator LoadConstantToStack(this ILGenerator gen, ushort value)
        {
            gen.Emit(OpCodes.Ldc_I4, value);    // load constant (value)

            return gen;
        }

        public static ILGenerator LoadRegisterToStack(this ILGenerator gen, Register register)
        {
            gen.Emit(OpCodes.Ldloc_0);  // load RegisterBank

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

            return gen;
        }

        public static ILGenerator LoadMemoryToStack(this ILGenerator gen)
        {
            // gets the value at the top of the stack as an address
            gen.Emit(OpCodes.Stloc_3);  // store ushort
            gen.Emit(OpCodes.Ldloc_1);  // load MemoryController
            gen.Emit(OpCodes.Ldloc_3);  // load ushort

            gen.EmitCall(OpCodes.Call, ReadMemoryMethod, null);

            return gen;
        }

        public static ILGenerator LoadMemoryDoubleToStack(this ILGenerator gen)
        {
            // gets the value at the top of the stack as an address
            gen.Emit(OpCodes.Stloc_3);  // store ushort
            gen.Emit(OpCodes.Ldloc_1);  // load MemoryController
            gen.Emit(OpCodes.Ldloc_3);  // load ushort

            gen.EmitCall(OpCodes.Call, ReadDoubleMemoryMethod, null);

            return gen;
        }

        public static ILGenerator LoadPCToStackAndAdvance(this ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldarg_0);  // load ProcessorExecutionContext

            gen.EmitCall(OpCodes.Call, ReadAndAdvanceMethod, null);

            return gen;
        }

        public static ILGenerator LoadDoublePCToStackAndAdvance(this ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldarg_0);  // load ProcessorExecutionContext

            gen.EmitCall(OpCodes.Call, ReadDoubleAndAdvanceMethod, null);

            return gen;
        }

        public static ILGenerator WriteStackToRegister(this ILGenerator gen, Register register)
        {
            // load the last stack value to an appropriate local var and then back to the stack
            switch (register)
            {
                // byte
                case Register.A:
                case Register.F:
                case Register.B:
                case Register.C:
                case Register.D:
                case Register.E:
                case Register.H:
                case Register.L:
                    gen.Emit(OpCodes.Stloc_2);  // store byte
                    gen.Emit(OpCodes.Ldloc_0);  // load RegisterBank
                    gen.Emit(OpCodes.Ldloc_2);  // load byte
                    break;

                // ushort
                case Register.AF:
                case Register.BC:
                case Register.DE:
                case Register.HL:
                case Register.SP:
                case Register.PC:
                    gen.Emit(OpCodes.Stloc_3);  // store ushort
                    gen.Emit(OpCodes.Ldloc_0);  // load RegisterBank
                    gen.Emit(OpCodes.Ldloc_3);  // load ushort
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

            return gen;
        }

        public static ILGenerator WriteStackToMemory(this ILGenerator gen)
        {
            // assumes that the first value on the stack is the value and the second - the address
            gen.Emit(OpCodes.Stloc_2);          // store byte
            gen.Emit(OpCodes.Stloc_S, (byte)4); // store int
            gen.Emit(OpCodes.Ldloc_1);          // load MemoryController
            gen.Emit(OpCodes.Ldloc_S, (byte)4); // load int
            gen.Emit(OpCodes.Ldloc_2);          // load byte

            gen.EmitCall(OpCodes.Call, WriteMemoryMethod, null);

            return gen;
        }

        public static ILGenerator WriteStackToMemory(this ILGenerator gen, int address)
        {
            // assumes that the value on the stack is the value
            gen.Emit(OpCodes.Stloc_2);          // store byte
            gen.Emit(OpCodes.Ldloc_1);          // load MemoryController
            gen.Emit(OpCodes.Ldc_I4, address);  // load constant (address)
            gen.Emit(OpCodes.Ldloc_2);          // load byte
            
            gen.EmitCall(OpCodes.Call, WriteMemoryMethod, null);

            return gen;
        }

        public static ILGenerator WriteDoubleStackToMemory(this ILGenerator gen)
        {
            // assumes that the first value on the stack is the value and the second - the address
            gen.Emit(OpCodes.Stloc_3);          // store ushort
            gen.Emit(OpCodes.Stloc_S, (byte)4); // store int
            gen.Emit(OpCodes.Ldloc_1);          // load MemoryController
            gen.Emit(OpCodes.Ldloc_S, (byte)4); // load int
            gen.Emit(OpCodes.Ldloc_3);          // load ushort

            gen.EmitCall(OpCodes.Call, WriteDoubleMemoryMethod, null);

            return gen;
        }

        public static ILGenerator WriteDobuleStackToMemory(this ILGenerator gen, int address)
        {
            // assumes that the value on the stack is the value
            gen.Emit(OpCodes.Stloc_3);          // store ushort
            gen.Emit(OpCodes.Ldloc_1);          // load MemoryController
            gen.Emit(OpCodes.Ldc_I4, address);  // load constant (address)
            gen.Emit(OpCodes.Ldloc_3);          // load ushort
            
            gen.EmitCall(OpCodes.Call, WriteDoubleMemoryMethod, null);

            return gen;
        }
    }
}
