using Fluxor;
using FluxorLearning.Client.Features.Counter.Store;
using FluxorLearning.Shared;
using System.Net.Http.Json;

namespace FluxorLearning.Client.Features.Weather.Store;

public record WeatherState(bool Initialized, bool Loading, WeatherForecast[] Forecasts);

public class WeatherFeature : Feature<WeatherState>
{
    public override string GetName() => "Weather";

    protected override WeatherState GetInitialState()
    {
        return new WeatherState(false, false, []);
    }
}

public class WeatherSetLoadingAction
{
    public bool Loading { get; }

    public WeatherSetLoadingAction(bool loading)
    {
        Loading = loading;
    }
}

public static class WeatherReducers
{
    [ReducerMethod]
    public static WeatherState OnSetForecasts(WeatherState state, WeatherSetForecastsAction action)
    {
        return state with
        {
            Forecasts = action.Forecasts,
            Loading = false
        };
    }

    [ReducerMethod(typeof(WeatherSetInitializedAction))]
    public static WeatherState OnSetInitialized(WeatherState state)
    {
        return state with
        {
            Initialized = true
        };
    }

    [ReducerMethod(typeof(WeatherLoadForecastsAction))]
    public static WeatherState OnLoadForecasts(WeatherState state)
    {
        return state with
        {
            Loading = true
        };
    }
}

public class WeatherEffects
{
    private readonly HttpClient Http;
    private readonly IState<CounterState> CounterState;

    public WeatherEffects(HttpClient http, IState<CounterState> counterState)
    {
        Http = http;
        CounterState = counterState;
    }

    [EffectMethod(typeof(WeatherLoadForecastsAction))]
    public async Task LoadForecasts(IDispatcher dispatcher)
    {
        await Task.Delay(1000);
        var forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
        dispatcher.Dispatch(new WeatherSetForecastsAction(forecasts!));
    }

    [EffectMethod(typeof(CounterIncrementAction))]
    public async Task LoadForecastsOnIncrement(IDispatcher dispatcher)
    {
        // every tenth increment
        await Task.Delay(0);
        if (CounterState.Value.CurrentCount % 2 == 0)
        {
            dispatcher.Dispatch(new WeatherLoadForecastsAction());
        }
    }

}

#region WeatherActions
public class WeatherSetInitializedAction { }
public class WeatherLoadForecastsAction { }
public class WeatherSetForecastsAction
{
    public WeatherForecast[] Forecasts { get; }

    public WeatherSetForecastsAction(WeatherForecast[] forecasts)
    {
        Forecasts = forecasts;
    }
}
#endregion