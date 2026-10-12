using System.Reflection;
using Microsoft.JavaScript.NodeApi;

namespace NodeApiRepro;

[JSExport]
public static class Main
{
    /// <summary>
    /// here we touch the dependency (Newtonsoft.Json.dll)
    /// </summary>
    static void TouchDependency()
    {
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { ok = true });
        Console.WriteLine(
            $"[repro] touched Newtonsoft.Json: {json} (managed thread {Environment.CurrentManagedThreadId})");
    }

    /// <summary>
    /// case 1: first touch happens on the JS thread.
    /// </summary>
    [JSExport]
    public static void TouchOnJsThread()
    {
        Console.WriteLine($"[repro] calling from JS thread {Environment.CurrentManagedThreadId}");
        TouchDependency();
    }

    /// <summary>
    /// case 2: first touch happens on a .NET thread-pool thread.
    /// </summary>
    [JSExport]
    public static void TouchOnPoolThread()
    {
        System.Threading.Tasks.Task.Run(() =>
        {
            Console.WriteLine($"[repro] running on thread-pool thread {Environment.CurrentManagedThreadId}");
            try
            {
                // Invoked via reflection so that the JIT (and therefore the dependency resolution)
                // cannot happen on the calling JS thread when the delegate is created.
                typeof(Main)
                    .GetMethod(nameof(TouchDependency), BindingFlags.NonPublic | BindingFlags.Static)!
                    .Invoke(null, null);
            }
            catch (Exception ex)
            {
                Dump(withMessagePrefix: "[repro]", ex);
                throw;
            }
        }).GetAwaiter().GetResult();
    }

    /// <summary>Prints the whole exception chain, including the inner exception that the
    /// FileLoadException hides.</summary>
    static void Dump(string withMessagePrefix, Exception ex)
    {
        for (var e = ex; e is not null; e = e.InnerException)
        {
            Console.WriteLine($"{withMessagePrefix} {e.GetType().FullName}: {e.Message}");
            Console.WriteLine($"{withMessagePrefix}   {e.StackTrace?.Replace("\n", "\n" + withMessagePrefix + "   ")}");
        }
    }
}
