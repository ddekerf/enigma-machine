using EnigmaMachine.Application.Commands;
using EnigmaMachine.Application.DependencyInjection;
using EnigmaMachine.Domain.Entities;
using EnigmaMachine.Domain.Factories;
using EnigmaMachine.Domain.Interfaces;
using EnigmaMachine.Domain.ValueObjects;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Accept enum values as strings (e.g. rotor types "I".."V") in JSON payloads
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

// Application services
builder.Services.AddApplication();

// Domain factories
builder.Services.AddTransient<Func<IPlugboard>>(_ => () => new Plugboard());
builder.Services.AddTransient<Func<IReflector>>(_ => () => new ReflectorB());
builder.Services.AddTransient<Func<RotorType[], char[], char[], IPlugboard, IReflector, IEnigmaMachine>>(
    _ => (rotors, ringSettings, initialPositions, plugboard, reflector) =>
        EnigmaMachineFactory.CreateEnigmaILeftToRight(rotors, ringSettings, initialPositions, plugboard, reflector));
builder.Services.AddTransient<ITextTransformer, DefaultTextTransformer>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();

app.MapGet("/", () =>
    Results.Text(
        "EnigmaMachine API is running. POST /api/enigma/process to encode text.",
        "text/plain"));

app.MapPost("/api/enigma/process", async (ProcessTextCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

app.Run();
