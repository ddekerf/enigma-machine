# EnigmaMachine.Domain

The `EnigmaMachine.Domain` project is part of the `EnigmaMachine` solution, which simulates the WWII Enigma encryption device. This project adheres to Clean Architecture and Domain-Driven Design (DDD) principles, focusing solely on the core business logic of the Enigma machine.

## Project Structure

The project is organized into the following folders:

- **Entities**: Contains the core domain entities that represent the main components of the Enigma machine.
  - `EnigmaMachine.cs`: Defines the `EnigmaMachine` class as the aggregate root, encapsulating the overall functionality of the machine.
  - `Plugboard.cs`: Represents the plugboard component, managing connections between letters.
  - `Reflector.cs`: Represents the reflector component, responsible for reflecting signals.
  - `Rotor.cs`: Represents a rotor in the Enigma machine, supporting an arbitrary number of rotors and handling rotation and signal processing.
  - `Factories`: Provides factory helpers such as `RotorFactory` and `EnigmaMachineFactory` for configuring historical machines.

- **ValueObjects**: Contains value objects that represent immutable concepts within the domain.
  - `Letter.cs`: Represents a single letter in the encryption process.
  - `PlugboardPair.cs`: Represents a pair of letters connected in the plugboard.
  - `RotorPosition.cs`: Represents the position of a rotor.
  - `RotorType.cs`: Enumerates available historical rotor types.

- **Interfaces**: Defines interfaces that abstract the behavior of the domain entities.
  - `IEnigmaMachine.cs`: Abstracts the behavior of the Enigma machine.
  - `IPlugboard.cs`: Abstracts the behavior of the plugboard.
  - `IRotor.cs`: Abstracts the behavior of a rotor.

- **Exceptions**: Contains domain-specific exceptions.
  - `DomainException.cs`: Represents exceptions that may occur within the domain layer.

## Purpose

The purpose of the `EnigmaMachine.Domain` project is to encapsulate the business logic related to the Enigma machine, ensuring that it remains isolated and testable. By adhering to DDD principles, the project aims to provide a clear and maintainable structure that can be easily extended in the future.

## Getting Started

To get started with the `EnigmaMachine.Domain` project, ensure that you have the necessary .NET SDK installed. You can build and run the project using standard .NET CLI commands.

This project does not include any infrastructure or UI code, focusing solely on the domain layer to maintain a clean separation of concerns.