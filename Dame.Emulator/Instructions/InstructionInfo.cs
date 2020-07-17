using System;
using System.IO;

namespace Dame.Emulator.Instructions
{
    public readonly struct InstructionInfo
    {
        public InstructionType Type { get; }

        public Operand FirstArgument { get; }
        public Operand SecondArgument { get; }

        
        public InstructionInfo(InstructionType type, Operand firstArgument, Operand secondArgument)
        {
            Type = type;
            FirstArgument = firstArgument;
            SecondArgument = secondArgument;
        }

        public override string ToString()
        {
            string instruction;
            int operands;

            switch (Type)
            {
                case InstructionType.Nop:           instruction = "NOP";    operands = 0; break;
                case InstructionType.Stop:          instruction = "STOP";   operands = 0; break;
                case InstructionType.Halt:          instruction = "HALT";   operands = 0; break;

                case InstructionType.Add:           instruction = "ADD";    operands = 2; break;
                case InstructionType.AddCarry:      instruction = "ADC";    operands = 2; break;
                case InstructionType.Subtract:      instruction = "SUB";    operands = 2; break;
                case InstructionType.SubtractCarry: instruction = "SBC";    operands = 2; break;

                case InstructionType.Increment:     instruction = "INC";    operands = 1; break;
                case InstructionType.Decrement:     instruction = "DEC";    operands = 1; break;
                
                case InstructionType.Xor:           instruction = "XOR";    operands = 2; break;
                case InstructionType.Or:            instruction = "OR";     operands = 2; break;
                case InstructionType.And:           instruction = "AND";    operands = 2; break;
                case InstructionType.Compare:       instruction = "CP";     operands = 2; break;

                case InstructionType.RotateLeft:        instruction = "RL";     operands = 1; break;
                case InstructionType.RotateLeftCarry:   instruction = "RLC";    operands = 1; break;
                case InstructionType.RotateRight:       instruction = "RR";     operands = 1; break;
                case InstructionType.RotateRightCarry:  instruction = "RRC";    operands = 1; break;

                case InstructionType.ShiftLeft:         instruction = "SLA";    operands = 1; break;
                case InstructionType.ShiftRight:        instruction = "SRA";    operands = 1; break;
                case InstructionType.ShiftRightLogical: instruction = "SRL";    operands = 1; break;

                case InstructionType.Swap:          instruction = "SWAP";   operands = 1; break;
                case InstructionType.TestBit:       instruction = "BIT";    operands = 2; break;
                case InstructionType.ResetBit:      instruction = "RES";    operands = 2; break;
                case InstructionType.SetBit:        instruction = "SET";    operands = 2; break;

                case InstructionType.Load:          instruction = "LD";     operands = 2; break;
                case InstructionType.Push:          instruction = "PUSH";   operands = 1; break;
                case InstructionType.Pop:           instruction = "POP";    operands = 1; break;

                case InstructionType.Jump:          instruction = "JP";     operands = 1; break;
                case InstructionType.JumpZ:         instruction = "JP Z";   operands = 1; break;
                case InstructionType.JumpNZ:        instruction = "JP NZ";  operands = 1; break;
                case InstructionType.JumpC:         instruction = "JP C";   operands = 1; break;
                case InstructionType.JumpNC:        instruction = "JP NC";  operands = 1; break;

                case InstructionType.JumpRelative:      instruction = "JR";     operands = 1; break;
                case InstructionType.JumpRelativeZ:     instruction = "JR Z";   operands = 1; break;
                case InstructionType.JumpRelativeNZ:    instruction = "JR NZ";  operands = 1; break;
                case InstructionType.JumpRelativeC:     instruction = "JR C";   operands = 1; break;
                case InstructionType.JumpRelativeNC:    instruction = "JR NC";  operands = 1; break;

                case InstructionType.Call:          instruction = "CALL";       operands = 1; break;
                case InstructionType.CallZ:         instruction = "CALL Z";     operands = 1; break;
                case InstructionType.CallNZ:        instruction = "CALL NZ";    operands = 1; break;
                case InstructionType.CallC:         instruction = "CALL C";     operands = 1; break;
                case InstructionType.CallNC:        instruction = "CALL NC";    operands = 1; break;

                case InstructionType.Return:          instruction = "RET";       operands = 0; break;
                case InstructionType.ReturnZ:         instruction = "RET Z";     operands = 0; break;
                case InstructionType.ReturnNZ:        instruction = "RET NZ";    operands = 0; break;
                case InstructionType.ReturnC:         instruction = "RET C";     operands = 0; break;
                case InstructionType.ReturnNC:        instruction = "RET NC";    operands = 0; break;

                case InstructionType.ReturnEnableInterrupts:    instruction = "RETI";   operands = 0; break;

                case InstructionType.EnableInterrupts:  instruction = "EI"; operands = 0; break;
                case InstructionType.DisableInterrupts: instruction = "DI"; operands = 0; break;

                case InstructionType.DAA:               instruction = "DAA";    operands = 0; break;
                case InstructionType.Complement:        instruction = "CPL";    operands = 0; break;
                case InstructionType.SetCarry:          instruction = "CCF";    operands = 0; break;
                case InstructionType.ComplementCarry:   instruction = "SCF";    operands = 0; break;

                default:
                    throw new InvalidDataException($"{nameof(Type)} is not a valid member of {nameof(InstructionType)}!");
            }

            if (operands == 0)
                return instruction;
            else if (operands == 1)
                return $"{instruction} {FirstArgument.ToString()}";
            else
                return $"{instruction} {FirstArgument.ToString()}, {SecondArgument.ToString()}";
        }
    }
}
