# 03-ReduxDevToolsIntegration (Integration with Redux Dev Tools)
This sample shows how to integrate with the [Redux dev tools] plugin for Google Chrome. It is recommended that you read both [Tutorial 1] and [Tutorial 2] first.

## Setting up the project
Once you have your project up and running, adding support for ReduxDevTools is simple. Edit the `Program` class within the `Client` project and add the Routing and Redux Dev Tools middlewares to the options.
```
var serviceProvider = new BrowserServiceProvider(services =>
{
	services.AddFluxor(options => options
		.UseDependencyInjection(typeof(Program).Assembly)
		.AddMiddleware<Blazor.Fluxor.ReduxDevTools.ReduxDevToolsMiddleware>()
		.AddMiddleware<Blazor.Fluxor.Routing.RoutingMiddleware>()
	);
});
```

Next we need to ensure the HTML on the client has the required Javascript to talk to the Redux dev tools. Edit the file `Shared\MainLayout.cshtml` and add the following code to the top of the file

```
@using Blazor.Fluxor.DevTools
@ReduxDevTools.Initialize()
```
## Required changes to state classes
Because the [Redux dev tools] implementation uses serialization to switch back to historial states it is necessary to create a public parameterless constructor on all of your state classes.

```
[Obsolete("For deserialization purposes only. Use the constructor with parameters")]
public CounterState() {}
```

## Temporary step
Currently there is no way in Blazor to instruct `JsonUtil.Deserialize()` to deserialize properties with private setters. Until this is possible you will need to ensure the setter visibility of all state properties is public. (See [Issue 705]).

## Subscribing to state changes
To ensure your component is re-rendered when state is changed in another component simply descend your components from `FluxorComponent`, like this `@inherits Blazor.Fluxor.Components.FluxorComponent`.

If you do not wish to descend from a specific base class you can instruct Fluxor to call your component's `StateHasChanged` method whenever its state changes, like this:

```
protected override void OnInit()
{
	base.OnInit();
	NameOfYourState.Changed(StateHasChanged)
}
```
This will register your component's `StateHasChanged` method to be called back whenever the state changes. There is no need to unsubscribe, Fluxor will no longer call back your component once it has been garbage collected.

[Redux dev tools]: <https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd>
[Tutorial 1]: <https://github.com/mrpmorris/blazor-fluxor/tree/master/samples/01-CounterSample>
[Tutorial 2]: <https://github.com/mrpmorris/blazor-fluxor/tree/master/samples/02-WeatherForecastSample>
[Issue 704]: <https://github.com/aspnet/Blazor/issues/704>
[Issue 705]: <https://github.com/aspnet/Blazor/issues/705>