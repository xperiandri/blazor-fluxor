# Blazor-Fluxor
Blazor-Fluxor is a low (zero) boilerplate Flux/Redux library for the new [Microsoft aspdotnet Blazor project]. 

The aim of Fluxor is to create a single-state store approach to front-end development in Blazor without the headaches typically associated with other implementations, such as the overwhelming amount of boiler-plate code required just to add a very basic feature.

## Installation
You can download the latest release / pre-release NuGet packages from the [official Blazor-Fluxor nuget page].

Install the dependencies and devDependencies and start the server.

## Getting started
The easiest way to get started is to look at the [Sample projects]. They are numbered in an order recommended for learning Blazor-Fluxor. Each will have a `readme` file that explains how the sample was created.

### Sample projects
More sample projects will be added as the framework develops.
  - [Counter sample] - Fluxorizes `Counter` page in the standard Visual Studio Blazor sample in order to show how to switch to a Redux/Flux pattern application using Fluxor.
  - [Effects sample] - Fluxorizes `FetchData` page in the standard Visual Studio Blazor sample in order to demonstrate asynchronous reactions to actions that are dispatched.
  - [Redux dev tools integration] - Demonstrates how to enable debugger integration for the [Redux dev tools] Chrome plugin.
  - [Custom Middleware] - Demonstrates how to create custom Middleware to intercept actions etc.

## What's new
### New in 0.12.0
 - Change versioning scheme to match the Blazor approach (increment minor version per release)
 - Make BrowserInterop an injected service
 - Ensure DisposableCallback can only be disposed once
 - Change Store.Features from IEnumerable<IFeature> to IReadonlyDictionary<string, Feature> for fast lookup and prevention of duplicate keys
 - Make Store.BeginInternalMiddlewareChange re-entrant
 - Fix NullReferenceException that could occur when Middleware returned null from IMiddleware.AfterDispatch
 - Added unit tests
### New in 0.0.11
  - Allow middleware to return tasks to dispatch from IMiddleware.AfterDispatch
  - Make methods of `Feature<TState>` virtual.
  - Upgraded to Blazor 0.4.0
### New in 0.0.10
  - Introduced IDispatcher for dispatching actions from Blazor components so that the whole IStore isn't required.
  - Introduced IState for providing feature state to Blazor components so that the entire IFeature<T> doesn't need to be referenced.
### New in 0.0.9
  - Renamed `Handle` to `HandleAsync` in effects
  - Added source docs
### New in 0.0.8
  - Added an example showing how to create Middleware modules for Fluxor
  - Fixed a bug where components were not displaying state updates when actions effecting their state were dispatched from another component
  
### New in 0.0.7
  - Renamed IStoreMiddleware to IMiddleware
  - Allow middleware to veto the dispatching of actions
  - Allow middleware to declare Javascript it needs to be added into the site's html page
  - Add routing middleware
  - Exclude auto-discovery of features / reducers / effects in namespaces that contain a class that implements IMiddleware
  - Auto register features / reducers / effects for classes in the same namespace (or below) of any class added with Options.AddMiddleware

### New in 0.0.6
  - Changed the signature of IStore.Dispatch to IStore.DispatchAsync
  - Upgraded to latest version of Blazor (0.3.0)

### New in 0.0.5
  - Changed the signature of ServiceCollection.AddFluxor to pass in an Options object
  - Added support for Redux Dev Tools
  - Added support for adding custom Middleware
  
### New in 0.0.4
  - Changed side-effects to return an array of actions to dispatch rather than limiting it to a single action
  
### New in 0.0.3
  - Added side-effects for calling out to async routines such as HTTP requests
  - Added a sample application to the [Sample projects]
  
### New in 0.0.2
  - Automatic discovery of store, features, and reducers via dependency injection.

### New in 0.0.1
  - Basic store / feature / reducer implementation
  
# License
MIT

   [Official Blazor-Fluxor nuget page]: <https://www.nuget.org/packages/Blazor.Fluxor>
   [Microsoft aspdotnet blazor project]: <https://github.com/aspnet/Blazor>
   [Counter sample]: <https://github.com/mrpmorris/blazor-fluxor/tree/master/samples/01-CounterSample>
   [Effects sample]: <https://github.com/mrpmorris/blazor-fluxor/tree/master/samples/02-WeatherForecastSample>
   [Redux dev tools integration]: <https://github.com/mrpmorris/blazor-fluxor/tree/master/samples/03-ReduxDevToolsIntegration>
   [Custom Middleware]: <https://github.com/mrpmorris/blazor-fluxor/tree/master/samples/04-MiddlewareSample>
   [Sample projects]: <https://github.com/mrpmorris/blazor-fluxor/tree/master/samples>
