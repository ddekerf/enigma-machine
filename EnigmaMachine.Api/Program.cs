using EnigmaMachine.Application.Commands;
using EnigmaMachine.Application.DependencyInjection;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Application services
builder.Services.AddApplication();

// Domain factories
builder.Services.AddTransient<Func<IPlugboard>>(_ => () => new Plugboard());
builder.Services.AddTransient<Func<IReflector>>(_ => () => new ReflectorB());
builder.Services.AddTransient<Func<RotorType[], char[], char[], IPlugboard, IReflector, IEnigmaMachine>>(
    _ => (rotors, ringSettings, initialPositions, plugboard, reflector) =>
        EnigmaMachineFactory.CreateEnigmaILeftToRight(rotors, ringSettings, initialPositions, plugboard, reflector));

var app = builder.Build();

app.MapPost("/api/enigma/process", async (ProcessTextCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

app.Run();
