using System;
using System.IO;
using FimCore.Interfaces;
using FimCore.Services;

public class Program
{
    public static void Main(string[] args)
    {
        IHashCalculator calculator = new Sha256HashCalculator();
        IBaselineRepository repository = new CsvBaselineRepository("baselines.csv");
        FimEngine engine = new FimEngine(calculator, repository);
    }
}