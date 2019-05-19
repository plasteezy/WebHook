using System.Threading.Tasks;

namespace WebHook.Common.Lib
{
    public delegate Task PipelineDelegate<in T>(T e);
}