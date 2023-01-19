using System.Globalization;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace SmartMeter.Monitor;

public class SmartMeterView
{
    private const string titleLayout = "Title";
    private const string timestampLayout = "Timestamp";
    private const string l1Layout = "L1";
    private const string l2Layout = "L2";
    private const string l3Layout = "L3";
    private const string summaryLayout = "Summary";

    private Layout HeaderLayout()
    {
        return new Layout()
            .Size(3)
            .SplitColumns(
                new Layout(titleLayout),
                new Layout(timestampLayout));
    }

    private Layout BodyLayout()
    {
        var bodyLayout = new Layout()
            .SplitRows(
                new Layout()
                    .Size(4)
                    .SplitColumns(
                        new Layout(l1Layout),
                        new Layout(l2Layout),
                        new Layout(l3Layout)),
                new Layout(summaryLayout)
            );
        return bodyLayout;
    }

    private IRenderable PhaseContent(string header, string voltage, string current)
    {
        var row = new Rows(
            new Markup($"Voltage (V) {voltage}").Centered(),
            new Markup($"Current (A) {current}").Centered()
        );

        return new Panel(Align.Center(row, VerticalAlignment.Middle))
        {
            Header = new PanelHeader(header, Justify.Center),
            Border = BoxBorder.Rounded,
            Expand = true
        };
    }

    public void Render(SmartMeterData data)
    {
        var layout = new Layout()
            .SplitRows(
                HeaderLayout(),
                BodyLayout()
            );


        var titlePanel = new Panel(new Markup("[bold]Smart Meter Data Overview[/]").Centered());
        titlePanel.Expand = true;
        titlePanel.Border = BoxBorder.Rounded;

        var timestampPanel =
            new Panel(new Markup($"[bold] Last Updated: {data.TimeStamp.ToString(CultureInfo.InvariantCulture)}[/]")
                .Centered());
        timestampPanel.Expand = true;
        timestampPanel.Border = BoxBorder.Rounded;

        var grid = new Grid();
        grid
            .AddColumn()
            .AddColumn();

        grid.AddRow(
            new Text("Power+ (W)").RightJustified(),
            new Text(data.ActivePowerPlusInWatt.ToString()).LeftJustified());
        grid.AddRow(
            new Text("Power- (W)").RightJustified(),
            new Text(data.ActivePowerMinusInWatt.ToString()).LeftJustified());
        grid.AddRow(
            new Text("Energy+ (Wh)").RightJustified(),
            new Text(data.ActiveEnergyPlusInAmpere.ToString()).LeftJustified());
        grid.AddRow(
            new Text("Energy- (Wh)").RightJustified(),
            new Text(data.ActiveEnergyMinusInAmpere.ToString()).LeftJustified());
        grid.AddRow(
            new Text("R-Energy+ (Wh)").RightJustified(),
            new Text(data.ReactiveEnergyPlusInWattHours.ToString()).LeftJustified());
        grid.AddRow(
            new Text("R-Energy- (Wh)").RightJustified(),
            new Text(data.ReactiveEnergyMinusInWattHours.ToString()).LeftJustified());
        grid.AddRow(
            new Text("Power Factor").RightJustified(),
            new Text(data.PowerFactor.ToString()).LeftJustified());

        var overallPanel = new Panel(Align.Center(grid))
        {
            Header = new PanelHeader("Summary", Justify.Center),
            Border = BoxBorder.Rounded,
            Expand = true
        };

        layout[titleLayout].Update(titlePanel);
        layout[timestampLayout].Update(timestampPanel);
        layout[l1Layout].Update(PhaseContent("L1", data.VoltageV1.ToString(CultureInfo.InvariantCulture),
            data.CurrentA1.ToString(CultureInfo.InvariantCulture)));
        layout[l2Layout].Update(PhaseContent("L2", data.VoltageV2.ToString(CultureInfo.InvariantCulture),
            data.CurrentA2.ToString(CultureInfo.InvariantCulture)));
        layout[l3Layout].Update(PhaseContent("L3", data.VoltageV3.ToString(CultureInfo.InvariantCulture),
            data.CurrentA3.ToString(CultureInfo.InvariantCulture)));
        layout[summaryLayout].Update(overallPanel);

        AnsiConsole.Write(layout);
    }
}