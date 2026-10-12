This is a minimal repo that reproduces the issue : the default node-dotnet-api's resolving assembly policy throws on .NET Task Pool thread.


## Preparation

```bash
dotnet restore
dotnet publish
npm i   
```


## Run the js script

Running on `JS thread` leads to success:
```bash
npm run on-js                                 

> resolve-assemlby-problem-demo@1.0.0 on-js
> node index.mjs js

[repro] calling from JS thread 1
[repro] touched Newtonsoft.Json: {"ok":true} (managed thread 1)
[js] RESULT: no exception
```

However, running on `.NET Task Pool` results in failure:
```bash
npm run on-pool


[repro] running on thread-pool thread 4
[repro] System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.
[repro]      at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
[repro]      at System.Reflection.MethodBase.Invoke(Object obj, Object[] parameters)
[repro]      at NodeApiRepro.Main.<>c.<TouchOnPoolThread>b__2_0() in ...\repro-a-lazy-dependency\Main.cs:line 42
[repro] System.IO.FileLoadException: Could not load file or assembly 'Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed'. An operation is not legal in the current state. (0x80131509)
[repro]      at NodeApiRepro.Main.TouchDependency()
[repro]      at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
[repro]      at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
[repro] Microsoft.JavaScript.NodeApi.JSInvalidThreadAccessException: There is no active JS value scope.
Current thread: #4 ".NET TP Worker". Consider using the synchronization context to switch to the JS thread.
[repro]      at Microsoft.JavaScript.NodeApi.JSValueScope.get_Current()
[repro]      at Microsoft.JavaScript.NodeApi.JSValue.GetCurrentRuntime(napi_env& env)
[repro]      at Microsoft.JavaScript.NodeApi.JSValue.CreateStringUtf16(String value)
[repro]      at Microsoft.JavaScript.NodeApi.JSValue.op_Implicit(String value)
[repro]      at Microsoft.JavaScript.NodeApi.DotNetHost.ManagedHost.OnResolvingAssembly(AssemblyLoadContext loadContext, AssemblyName assemblyInfo)
[repro]      at System.Runtime.Loader.AssemblyLoadContext.GetFirstResolvedAssemblyFromResolvingEvent(AssemblyName assemblyName)
[repro]      at System.Runtime.Loader.AssemblyLoadContext.ResolveUsingEvent(AssemblyName assemblyName)
[repro]      at System.Runtime.Loader.AssemblyLoadContext.ResolveUsingResolvingEvent(IntPtr gchManagedAssemblyLoadContext, AssemblyName assemblyName)
[undefined] RESULT: Could not load file or assembly 'Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed'. An operation is not legal in the current state. (0x80131509)
``