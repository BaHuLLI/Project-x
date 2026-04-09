using ProjectXProDash.Core;

namespace ProjectXProDash.UnitTests;

public sealed class RelayCommandTests
{
    [Fact]
    public void ExecuteUsesPredicateAndDelegate()
    {
        object? executedParameter = null;
        var command = new RelayCommand(
            parameter => executedParameter = parameter,
            parameter => parameter is string value && value == "run");

        Assert.False(command.CanExecute("skip"));
        Assert.True(command.CanExecute("run"));

        command.Execute("run");

        Assert.Equal("run", executedParameter);
    }

    [Fact]
    public void RaiseCanExecuteChangedRaisesEvent()
    {
        var command = new RelayCommand(_ => { });
        var wasRaised = false;
        command.CanExecuteChanged += (_, _) => wasRaised = true;

        command.RaiseCanExecuteChanged();

        Assert.True(wasRaised);
    }
}

