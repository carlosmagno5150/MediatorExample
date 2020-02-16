using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Custom.Cqrs.Commands
{
    public interface ICommand : IRequest
    {
    }
    public abstract class Command : ICommand
    {
    }
}
