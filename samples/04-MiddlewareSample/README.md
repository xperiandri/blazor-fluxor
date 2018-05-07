# 01-MiddlewareSample
This sample shows how to write Middleware for Fluxor

## Creating your middleware
1. In the Client project create a new folder `Store\Middlewares\AnExample`
2. Create a new class `AnExampleMiddleware` and descend it from the `Blaxor.Fluxor.Middleware` class

## Consuming your middleware
Fluxor will not automatically discover and install Middlewares. These must be registered manually. In the Client's `Program` class change the serviceProvider configuration like so

```
var serviceProvider = new BrowserServiceProvider(services =>
{
	services.AddFluxor(options =>
		options
			.AddMiddleware<AnExampleMiddleware>()
			.UseDependencyInjection(typeof(Program).Assembly)
	);
});
```

Features, Reducers, and Effects in the same assembly + namespace (or sub-namespace) as a class implementing IMiddleware will be blacklisted from automatic registration when using `options.UseDependencyInjection`.

When you call `options.AddMiddleware<T>()` then classes in the same assembly + namespace (or sub-namespace) will be whitelisted, and therefore autoatically discovered by dependency injection because you have whitelisted the middleware.

For a more comprehensive example of writing middleware for Fluxor look at the [Redux Devtools Middleware] classes.

[Redux Devtools Middleware]: <https://github.com/mrpmorris/blazor-fluxor/tree/master/src/Blazor.Fluxor/ReduxDevTools>