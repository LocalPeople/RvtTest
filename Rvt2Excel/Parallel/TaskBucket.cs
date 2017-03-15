using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rvt2Excel.Parallel
{
    abstract class TaskBucket<TResult>
    {
        public Task<TResult> TaskAsync { get; protected set; }

        public abstract void ExecuteAsync();
    }

    static class TaskBucket
    {
        public static void WaitAll<T>(params TaskBucket<T>[] buckets)
        {
            Task[] tasks = buckets.Select(b => b.TaskAsync).ToArray();
            Task.WaitAll(tasks);
        }
    }
}
