namespace SmartMeter.Monitor;

public class SmartMeterData
{
    public DateTime TimeStamp { get; set; }
    public float ActivePowerPlusInWatt { get; init; }
    public float ActivePowerMinusInWatt { get; init; }
    public float ActiveEnergyPlusInAmpere{ get; init; }
    public float ActiveEnergyMinusInAmpere { get; init; }
    public float ReactiveEnergyPlusInWattHours { get; init; }
    public float ReactiveEnergyMinusInWattHours { get; init; }
    public float VoltageV1 { get; init; }
    public float VoltageV2 { get; init; }
    public float VoltageV3 { get; init; }
    public float CurrentA1 { get; init; }
    public float CurrentA2 { get; init; }
    public float CurrentA3 { get; init; }
    public int PowerFactor { get; init; }
}