﻿namespace Application.Interfaces
{
    public interface ICommand
    {
    }

    public interface ICommand<TResponse> : ICommand
    {
    }
}
