using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AwaitableHelper : CustomYieldInstruction
{
	private readonly Func<bool> condition;

	public AwaitableHelper(Func<bool> condition)
	{
		this.condition = condition;
	}

	public override bool keepWaiting => !condition();

	public static async Task WaitForConditionAsync(Func<bool> condition, CancellationToken token = default)
	{
		var awaitable = new AwaitableHelper(condition);
		while (awaitable.keepWaiting)
		{
			token.ThrowIfCancellationRequested();
			await Task.Yield(); // Allows other tasks to run while waiting
		}
	}
}