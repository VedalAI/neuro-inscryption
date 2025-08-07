using System.Threading;
using Cysharp.Threading.Tasks;
using NeuroSdk.Actions;

namespace NeuroInscryption.Communication.Actions;

public abstract class AsyncNeuroAction : NeuroAction, IAsyncNeuroAction
{
    protected override string Description => "";

    private readonly CancellationToken _externalCt;
    private readonly CancellationTokenSource _cts;
    public UniTask ReturningTask { get; }
    public abstract bool IsApplicable { get; }

    protected AsyncNeuroAction(CancellationToken ct = default)
    {
        _externalCt = ct;
        _cts = new CancellationTokenSource();
        ReturningTask = _cts.Token.ToUniTask().Item1;
        
        _externalCt.Register(_cts.Cancel);
    }

    protected abstract UniTask ExecuteAsync(CancellationToken ct);
    
    protected override void Execute()
    {
        ExecuteAsync(_externalCt).ContinueWith(_cts.Cancel).Forget();
    }
}

public abstract class AsyncNeuroAction<T> : NeuroAction<T>, IAsyncNeuroAction
{
    protected override string Description => "";
    
    private readonly CancellationToken _externalCt;
    private readonly CancellationTokenSource _cts;
    public UniTask ReturningTask { get; }
    public abstract bool IsApplicable { get; }

    protected AsyncNeuroAction(CancellationToken ct = default)
    {
        _externalCt = ct;
        _cts = new CancellationTokenSource();
        ReturningTask = _cts.Token.ToUniTask().Item1;
        
        _externalCt.Register(_cts.Cancel);
    }

    protected abstract UniTask ExecuteAsync(T? parsedData, CancellationToken ct);
    
    protected override void Execute(T? parsedData)
    {
        ExecuteAsync(parsedData, _externalCt).ContinueWith(_cts.Cancel).Forget();
    }
}

public abstract class AsyncNeuroActionS<T>(CancellationToken ct = default) : AsyncNeuroAction<T?>(ct) where T : struct
{
}

public interface IAsyncNeuroAction : INeuroAction
{
    bool IsApplicable { get; }
    UniTask ReturningTask { get; }
}