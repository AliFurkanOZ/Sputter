using System;

namespace SputterNewApp.Core.Registers
{
    /// <summary>
    /// RegisterId values are the numeric holding-register addresses (start address for Float32 which spans 2 registers).
    /// Note: PLC 'D' prefix is intentionally ignored.
    /// </summary>
    public enum RegisterId : ushort
    {
        DC_Control_Command = 0, // 0=Stop, 1=Start
        DC_Step1_SetW = 1, //Int16
        DC_Step1_Ramp = 2, //Int16
        DC_Step1_Dwell = 3, //Int16
        DC_Step2_SetW = 4, //Int16
        DC_Step2_Ramp = 5, //Int16
        DC_Step2_Dwell = 6,  //Int16
        DC_OffRamp = 7, //Int16
        DC_Set_Voltage = 8, //Int16
        DC_Set_Current = 9, //Int16

        D1ActualFlow = 10, //2 regs; word order CDAB; big-endian bytes
        D1ReadSetPoint = 12, //2 regs; word order CDAB; big-endian bytes
        D1WriteSetPoint = 14, //2 regs; word order CDAB; big-endian bytes
        D1ReadFullScale = 16, //2 regs; word order CDAB; big-endian bytes
        D1WriteFullScale = 18, //2 regs; word order CDAB; big-endian bytes
        D1ReadGasIndex = 20,  //1 reg Int16
        D1WriteGasIndex = 21, //1 reg Int16
        D1ReadValvePosition = 22, //1 reg Int16; // 2=Close, 1=Open
        D1WriteValvePosition = 23, //1 reg Int16; // 2=Close, 1=Open
        D1ReadFlowUnit = 24, //1 reg Int16
        D1WriteFlowUnit = 25, //1 reg Int16
        D2ActualFlow = 26,
        D2ReadSetPoint = 28,
        D2WriteSetPoint = 30,
        D2ReadFullScale = 32,
        D2WriteFullScale = 34,
        D2ReadGasIndex = 36,
        D2WriteGasIndex = 37,
        D2ReadValvePosition = 38,
        D2WriteValvePosition = 39,
        D2ReadFlowUnit = 40,
        D2WriteFlowUnit = 41,
        D3ActualFlow = 42,
        D3ReadSetPoint = 44,
        D3WriteSetPoint = 46,
        D3ReadFullScale = 48,
        D3WriteFullScale = 50,
        D3ReadGasIndex = 52,
        D3WriteGasIndex = 53,
        D3ReadValvePosition = 54,
        D3WriteValvePosition = 55,
        D3ReadFlowUnit = 56,
        D3WriteFlowUnit = 57,
        D4ActualFlow = 58,
        D4ReadSetPoint = 60,
        D4WriteSetPoint = 62,
        D4ReadFullScale = 64,
        D4WriteFullScale = 66,
        D4ReadGasIndex = 68,
        D4WriteGasIndex = 69,
        D4ReadValvePosition = 70,
        D4WriteValvePosition = 71,
        D4ReadFlowUnit = 72,
        D4WriteFlowUnit = 73,
        D5ActualFlow = 74,
        D5ReadSetPoint = 76,
        D5WriteSetPoint = 78,
        D5ReadFullScale = 80,
        D5WriteFullScale = 82,
        D5ReadGasIndex = 84,
        D5WriteGasIndex = 85,
        D5ReadValvePosition = 86,
        D5WriteValvePosition = 87,
        D5ReadFlowUnit = 88,
        D5WriteFlowUnit = 89,

        // GAP (90-99)

        // MANUAL CONTROL START COMMANDS (Int16; 100-119 // 1=Active, 0=Inactive)
        Rotation_Motor_Start = 100,
        Rezistans_Start = 101,
        RF1_Power = 102,
        RF2_Power = 103,
        DC_Power = 104,
        MatchBox_Power = 105,
        Scroll_Vakum_Start = 106,
        Turbo_EVP_Vakum_Start = 107,
        Chiller_Start = 108,
        Gatevalve_Start = 109,
        Chamber_Valf_Start = 110,
        Soft_Turbo_Valf_Start = 111,
        RF1_Shutter_Start = 112,
        DC_Shutter_Start = 113,
        RF2_Shutter_Start = 114,
        //Aşağısı şuan kullanılmıyor
        Slot_Valf4_Start = 115,
        Slot_Valf5_Start = 116,
        Yedek1_Start = 117,
        Yedek2_Start = 118,
        Yedek3_Start = 119,
        Yedek4_Start = 120,

        // GAP (120-124)
        TorDegerAux = 123, //2 regs; word order CDAB; big-endian bytes
        AnalogVoltaj1 = 125, //2 regs; word order CDAB; big-endian bytes
        TorDeger = 127, //2 regs; word order CDAB; big-endian bytes

        // GAP(129)

        // RF1 CONTROL (130-141)
        RF1_Control_Command = 130, // 0=Stop, 1=Start
        RF1_Step1_SetW = 131, //Int16
        RF1_Step1_Ramp = 132, //Int16
        RF1_Step1_Dwell = 133, //Int16
        RF1_Step2_SetW = 134, //Int16
        RF1_Step2_Ramp = 135, //Int16
        RF1_Step2_Dwell = 136,  //Int16
        RF1_OffRamp = 137, //Int16
        RF1_FwP = 138, //Int16
        RF1_RefP = 139, //Int16
        RF1_Status = 140, //Int16
        RF1_ReadW = 141, //Int16

        // GAP (142-149)

        GateValve_Position = 150, //Int16
        RotationMotor_Rpm = 151, //Int16
        Turbo_Frequency = 152, //Int16
        GateValve_Speed = 153, //Int16
        GateValve_State = 154, //Int16 (0=Stop, 4998=Opening, 4999=Closing)

        // GAP (155-159)

        // DC CONTROL (160-222)
        DC_Read_VSet = 160,
        DC_Read_CSet = 161,
        DC_Read_PSet = 162,
        DC_Read_Voltage = 163,
        DC_Read_Current = 164,
        DC_Read_Power = 165,
        DC_Read_WorkMode = 166,
        DC_Read_HW = 170,
        DC_Interlock = 171,
        DC_Read_Out = 172,
        DC_Voltage_Ramp_Read = 184,
        DC_Current_Ramp_Read = 185,
        DC_Power_Ramp_Read = 186,
        DC_ReadBeeper = 190,
        DC_Read_Outp_Mode = 192,
        DC_Read_Pulse_Frq = 193,
        DC_Volt_Set = 210,
        DC_Current_Set = 211,
        DC_Power_Set = 212,
        DC_Set_Work_Mode = 213,
        DC_VoltageLimit_Set = 214,
        DC_Current_Limit_Set = 215,
        DC_Power_Limit_Set = 216,
        DC_Ramp_ActiveSet1 = 217,
        DC_Voltage_Ramp_Set = 218,
        DC_Current_Ramp_Set = 219,
        DC_Interface = 220,
        DC_Power_Ramp_Set = 221,
        DC_Set_Out = 222,
        DC_Status = 222, // 0=Stop, 1=Start PLC feedback status (alias for DC_Set_Out)

        Thickness1 = 230, //2 regs; word order CDAB; big-endian bytes
        Rate1 = 232, //2 regs; word order CDAB; big-endian bytes
        Life1 = 234, //2 regs; word order CDAB; big-endian bytes
        SerialNumber1 = 236, //2 regs,int32; word order CDAB; big-endian bytes
        Thickness2 = 238, //2 regs; word order CDAB; big-endian bytes
        Rate2 = 240, //2 regs; word order CDAB; big-endian bytes
        Life2 = 242, //2 regs; word order CDAB; big-endian bytes
        SerialNumber2 = 244, //2 regs,int32; word order CDAB; big-endian bytes
        Thickness3 = 246, //2 regs; word order CDAB; big-endian bytes
        Rate3 = 248, //2 regs; word order CDAB; big-endian bytes
        Life3 = 250, //2 regs; word order CDAB; big-endian bytes
        SerialNumber3 = 252, //2 regs,int32; word order CDAB; big-endian bytes

        Z_Ratio1 = 254, //2 regs; word order CDAB; big-endian bytes
        Density1 = 256, //2 regs; word order CDAB; big-endian bytes
        Tooling1 = 258, //2 regs; word order CDAB; big-endian bytes
        ZeroThickness1 = 260, //2 regs; word order CDAB; big-endian bytes
        Z_Ratio2 = 262, //2 regs; word order CDAB; big-endian bytes
        Density2 = 264, //2 regs; word order CDAB; big-endian bytes
        Tooling2 = 266, //2 regs; word order CDAB; big-endian bytes
        ZeroThickness2 = 268, //2 regs; word order CDAB; big-endian bytes
        Z_Ratio3 = 270, //2 regs; word order CDAB; big-endian bytes
        Density3 = 272, //2 regs; word order CDAB; big-endian bytes
        Tooling3 = 274, //2 regs; word order CDAB; big-endian bytes
        ZeroThickness3 = 276, //2 regs; word order CDAB; big-endian bytes

        // RF2 CONTROL (280-291)
        RF2_Control_Command = 280, // 0=Stop, 1=Start
        RF2_Step1_SetW = 281, //Int16
        RF2_Step1_Ramp = 282, //Int16
        RF2_Step1_Dwell = 283, //Int16
        RF2_Step2_SetW = 284, //Int16
        RF2_Step2_Ramp = 285, //Int16
        RF2_Step2_Dwell = 286,  //Int16
        RF2_OffRamp = 287, //Int16
        RF2_FwP = 288, //Int16
        RF2_RefP = 289, //Int16
        RF2_Status = 290, //Int16
        RF2_ReadW = 291, //Int16

        //Order Set Value: D292  Order_Instant Value:D294 Order Read_ Set Value:D295
        Ordel_SetValue = 292, //Int16
        Ordel_Instant_Value = 294, //Int16
        Ordel_Value = 295, //Int16

        EmergencyStop = 350 // 0=Not pressed, 1=Pressed
    }
}
