namespace GraphvizAvalonia.Impl;

internal abstract class Unit(double value)
{
    protected const double DipsPerInch = 96.0;
    protected const double PointsPerInch = 72.0;

    public double Value { get; } = value;

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}

internal sealed class Pts(double value) : Unit(value)
{
    public static explicit operator Pts(double points) => new(points);
    public static explicit operator Pts(string points) => new(double.Parse(points, System.Globalization.CultureInfo.InvariantCulture));
    public static explicit operator Pts(Dips dips) => new(dips.Value * (PointsPerInch / DipsPerInch));

    public static Pts operator -(Pts left, Pts right) => new(left.Value - right.Value);
}

internal sealed class Dips(double value) : Unit(value)
{
    public static explicit operator Dips(double dips) => new(dips);
    public static explicit operator Dips(Inches inches) => new(inches.Value * DipsPerInch);
    public static explicit operator Dips(Pts points) => new(points.Value * (DipsPerInch / PointsPerInch));

    public static Dips operator -(Dips left, Dips right) => new(left.Value - right.Value);
    public static Dips operator /(Dips left, int right) => new(left.Value / right);
}

internal sealed class Inches(double value) : Unit(value)
{
    public static explicit operator Inches(double inches) => new(inches);
    public static explicit operator Inches(Dips dips) => new(dips.Value / DipsPerInch);
    public static explicit operator Inches(Pts points) => new(points.Value / PointsPerInch);
}
