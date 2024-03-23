using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace ybwork.Assets
{
    public static class Extension
    {
        public static TaskAwaiter<T> GetAwaiter<T>(this T asyncOperation) where T : AsyncOperation
        {
            TaskCompletionSource<T> taskCompletionSource = new();
            asyncOperation.completed += (AsyncOperation obj) =>
            {
                taskCompletionSource.SetResult(obj as T);
            };
            return taskCompletionSource.Task.GetAwaiter();
        }
    }
}
