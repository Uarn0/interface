using System.Numerics;

internal class Program
{
    private static void Main(string[] args)
    {
        TestAPlusBSquare(new MyFrac(1, 3), new MyFrac(1, 6));
        TestAPlusBSquare(new MyComplex(1, 3), new MyComplex(1, 6));
        Console.ReadKey();

        MyComplex complex1 = new MyComplex(1, 3);  // 1 + 3i
        MyComplex complex2 = new MyComplex(1, 6);  // 1 + 6i

        Console.WriteLine(complex1.ToString());  // Виведе: 1 + 3i
        Console.WriteLine(complex2.ToString());  // Виведе: 1 + 6i

        MyComplex sumComplex = complex1.Add(complex2);
        Console.WriteLine(sumComplex.ToString());  // Виведе: 2 + 9i (результат додавання)

        Console.ReadKey();

        MyFrac fraction1 = new MyFrac(1, 3);  // дроб 1/3
        MyFrac fraction2 = new MyFrac(1, 6);  // дроб 1/6

        Console.WriteLine(fraction1.ToString());  // Виведе: 1/3
        Console.WriteLine(fraction2.ToString());  // Виведе: 1/6

        MyFrac sum = fraction1.Add(fraction2);
        Console.WriteLine(sum.ToString());  // Виведе: 1/2 (результат додавання)

        Console.WriteLine("MyFrac:");
        MyFrac result = fraction1.Divide(new MyFrac(0, 1));  // Ділення на 0
        Console.WriteLine(result);
        Console.WriteLine();
        Console.WriteLine("MyComplex:");
        var complex11 = new MyComplex(3, 4); // 3 + 4i
        var complexZero = new MyComplex(0, 0); // 0 + 0i

        var result1 = complex11.Divide(complexZero);
        Console.WriteLine(result1); // Цей рядок не буде виконаний через виняток// Ділення на 0
        Console.WriteLine(result1);
        Console.ReadKey();
    }


    static void TestAPlusBSquare<T>(T a, T b) where T : IMyNumber<T>
    {
        Console.WriteLine("=== Starting testing (a+b)^2=a^2+2ab+b^2 with a = " + a + ", b = " + b + " ===");
        T aPlusB = a.Add(b);
        Console.WriteLine("a = " + a);
        Console.WriteLine("b = " + b);
        Console.WriteLine("(a + b) = " + aPlusB);
        Console.WriteLine("(a+b)^2 = " + aPlusB.Multiply(aPlusB));
        Console.WriteLine(" = = = ");
        T curr = a.Multiply(a);
        Console.WriteLine("a^2 = " + curr);
        T wholeRightPart = curr;
        curr = a.Multiply(b);
        curr = curr.Add(curr); // ab + ab = 2ab

        // I'm not sure how to create constant factor "2" in more elegant way,
        // without knowing how IMyNumber is implemented
        Console.WriteLine("2*a*b = " + curr);
        wholeRightPart = wholeRightPart.Add(curr);
        curr = b.Multiply(b);
        Console.WriteLine("b^2 = " + curr);
        wholeRightPart = wholeRightPart.Add(curr);
        Console.WriteLine("a^2+2ab+b^2 = " + wholeRightPart);
        Console.WriteLine("=== Finishing testing (a+b)^2=a^2+2ab+b^2 with a = " + a + ", b = " + b + " ===");
    }


    static void TestSquaresDifference<T>(T a, T b) where T : IMyNumber<T>
    {
        Console.WriteLine("=== Testing (a-b)^2 = (a^2 - b^2)/(a+b) ===");
        var aMinusB = a.Subtract(b);
        Console.WriteLine("(a - b) = " + aMinusB);

        var left = aMinusB.Multiply(aMinusB);
        Console.WriteLine("(a - b)^2 = " + left);

        var aSquared = a.Multiply(a);
        var bSquared = b.Multiply(b);
        var right = aSquared.Subtract(bSquared).Divide(a.Add(b));

        Console.WriteLine("a^2 - b^2 / (a + b) = " + right);
        Console.WriteLine($"Validation: {left.Equals(right)}");
    }
}
public interface IMyNumber<T> where T : IMyNumber<T>
{
    T Add(T other);
    T Subtract(T other);
    T Multiply(T other);
    T Divide(T other);
}
public class MyFrac : IMyNumber<MyFrac>
{
    public MyFrac(string fraction)
    {
        if (string.IsNullOrWhiteSpace(fraction))
            throw new ArgumentException("Input string cannot be null or empty.");

        var parts = fraction.Split('/');
        if (parts.Length != 2 || !BigInteger.TryParse(parts[0], out var num) || !BigInteger.TryParse(parts[1], out var denom))
        {
            throw new ArgumentException($"Invalid fraction format: {fraction}");
        }

        Numerator = num;
        Denominator = denom;

        if (Denominator == 0)
        {
            throw new DivideByZeroException("Denominator cannot be zero.");
        }

        Simplify();
    }
    public BigInteger Numerator { get; private set; }
    public BigInteger Denominator { get; private set; }

    public MyFrac(BigInteger numerator, BigInteger denominator)
    {
        if (denominator == 0)
        {
            throw new DivideByZeroException("Denominator cannot be zero");
        }
        Numerator = numerator;
        Denominator = denominator;
        Simplify();
    }
    private void Simplify()
    {
        BigInteger gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator);
        Numerator /= gcd;
        Denominator /= gcd;
        if(Denominator == 0)
        {
            Numerator = -Numerator;
            Denominator = -Denominator;
        }
    }
    public int CompareTo(MyFrac other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        var left = this.Numerator * other.Denominator;
        var right = other.Numerator * this.Denominator;

        return left.CompareTo(right);
    }
    public MyFrac Add(MyFrac other)
    {
        return new MyFrac(this.Numerator * other.Denominator + other.Numerator * this.Denominator, this.Denominator * other.Denominator);
    }
    public MyFrac Subtract(MyFrac other)
    {
        return new MyFrac(this.Numerator * other.Denominator - other.Numerator * this.Denominator, this.Denominator * other.Denominator);
    }
    public MyFrac Multiply(MyFrac other)
    {
        return new MyFrac(this.Numerator * other.Numerator, this.Denominator * other.Denominator);
    }
    public MyFrac Divide(MyFrac other)
    {
        try
        {
            if (other.Numerator == 0)
                throw new DivideByZeroException("Cannot divide by a fraction with a numerator of zero.");
            return new MyFrac(this.Numerator * other.Denominator, this.Denominator * other.Numerator);
        }catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new MyFrac(0, 1); 
        }
        
    }
    public override string ToString()
    {
        return Denominator == 1 ? Numerator.ToString() : $"{Numerator}/{Denominator}";
    }

}

public class MyComplex : IMyNumber<MyComplex>
{
    public double Real { get; private set; }
    public double Imaginary { get; private set; }

    public MyComplex(string complex)
    {
        if (string.IsNullOrWhiteSpace(complex))
            throw new ArgumentException("Input string cannot be null or empty.");

        complex = complex.Replace(" ", "").Replace("i", "");
        var parts = complex.Split(new char[] { '+', '-' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2)
        {
            Real = double.Parse(parts[0]);
            Imaginary = double.Parse(complex.Contains("-") ? $"-{parts[1]}" : parts[1]);
        }
        else if (parts.Length == 1)
        {
            if (complex.Contains("i"))
            {
                Real = 0;
                Imaginary = double.Parse(parts[0]);
            }
            else
            {
                Real = double.Parse(parts[0]);
                Imaginary = 0;
            }
        }
        else
        {
            throw new ArgumentException($"Invalid complex number format: {complex}");
        }
    }

    public MyComplex(double real, double imaginary)
    {
        Real = real;
        Imaginary = imaginary;
    }

    public MyComplex Add(MyComplex other)
    {
        return new MyComplex(this.Real + other.Real, this.Imaginary + other.Imaginary);
    }

    public MyComplex Subtract(MyComplex other)
    {
        return new MyComplex(this.Real - other.Real, this.Imaginary - other.Imaginary);
    }

    public MyComplex Multiply(MyComplex other)
    {
        return new MyComplex(
            this.Real * other.Real - this.Imaginary * other.Imaginary,
            this.Real * other.Imaginary + this.Imaginary * other.Real
        );
    }

    public MyComplex Divide(MyComplex other)
    {
        try
        {
            double divisor = other.Real * other.Real + other.Imaginary * other.Imaginary;
            if (divisor == 0)
            {
                throw new DivideByZeroException("Cannot divide by a complex number with a magnitude of zero.");
            }
            return new MyComplex(
                (this.Real * other.Real + this.Imaginary * other.Imaginary) / divisor,
                (this.Imaginary * other.Real - this.Real * other.Imaginary) / divisor
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new MyComplex(0, 1);
        }
    }

    public override string ToString()
    {
        return Imaginary >= 0
            ? $"{Real} + {Imaginary}i"
            : $"{Real} - {-Imaginary}i";
    }
}
