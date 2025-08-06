using System.Threading;
using Cysharp.Threading.Tasks;
using NeuroSdk.Actions;

namespace NeuroInscryption.Communication.Actions;

public abstract class AsyncNeuroAction : NeuroAction
{
    private readonly CancellationToken _externalCt;
    private readonly CancellationTokenSource _cts;
    public readonly UniTask ReturningTask; 

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

public abstract class AsyncNeuroAction<T> : NeuroAction<T>
{
    private readonly CancellationToken _externalCt;
    private readonly CancellationTokenSource _cts;
    public readonly UniTask ReturningTask; 

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