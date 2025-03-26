using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Devlooped;

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    Console.InputEncoding = Console.OutputEncoding = Encoding.UTF8;

args = [.. ExpandResponseFiles(args)];

#if DEBUG
if (args.Contains("--debug"))
{
    Debugger.Launch();
    args = args.Where(x => x != "--debug").ToArray();
}
#endif

if (args.Contains("-?"))
    args = [.. args.Select(x => x == "-?" ? "-h" : x)];

var app = App.Create();

if (args.Contains("--version"))
{
    app.ShowVersion();
#if DEBUG
    await app.ShowUpdatesAsync(args);
#endif
    return 0;
}

#if DEBUG
return await app.RunAsync(args);
#else
return await app.RunWithUpdatesAsync(args);
#endif

static IEnumerable<string> ExpandResponseFiles(IEnumerable<string> args)
{
    foreach (var arg in args)
    {
        if (arg.StartsWith('@'))
        {
            var filePath = arg[1..];

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Response file not found: {filePath}");

            foreach (var line in File.ReadAllLines(filePath))
            {
                yield return line;
            }
        }
        else
        {
            yield return arg;
        }
    }
}