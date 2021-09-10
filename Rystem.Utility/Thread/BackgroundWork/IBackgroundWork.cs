using System;
using System.Threading.Tasks;

namespace Rystem.Background
{
    public interface IBackgroundWork
    {
        Task ActionToDoAsync();
        Task OnException(Exception exception);
    }
}