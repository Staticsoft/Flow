# Staticsoft Flow

A .NET 8 workflow orchestration library that enables building resilient, distributed job processing systems with support for long-running operations, external input points, and automatic retry mechanisms.

## Overview

Flow is designed to handle complex workflows that may involve:
- **Long-running operations** that can be paused and resumed
- **External input points** that require data from external sources
- **Distributed processing** across multiple workers
- **Automatic retry** and error handling
- **Progress tracking** for multi-step jobs

## Key Features

- **Job Orchestration**: Define complex workflows as jobs composed of multiple operations
- **Operation Isolation**: Each operation is independently tracked and can be retried
- **External Input Support**: Built-in support for workflows that require external data input
- **Multiple Deployment Models**: Support for both local execution and AWS Lambda
- **Progress Tracking**: Real-time status updates for running jobs
- **Fault Tolerance**: Automatic retry mechanisms and graceful error handling
- **Storage Abstraction**: Pluggable storage backend for job state persistence

## Architecture

### Core Abstractions

- **Job**: A workflow composed of multiple operations that produces an output from an input
- **Operation**: An atomic unit of work that can be executed independently
- **ExternalInput**: A point in the workflow that requires external data to proceed
- **Handler**: The execution logic for jobs and operations

### Components

- **Flow**: Core library with workflow orchestration logic
- **Flow.Lambda**: AWS Lambda integration for serverless execution
- **Flow.Local**: Local execution environment for development and testing

## Installation

```bash
# Core library
dotnet add package Staticsoft.Flow

# For AWS Lambda deployment
dotnet add package Staticsoft.Flow.Lambda

# For local development
dotnet add package Staticsoft.Flow.Local
```

## Quick Start

### 1. Define an Operation

```csharp
public class CalculateSquareOperation : Operation<CalculateSquareInput, CalculateSquareOutput>
{
    public async Task<CalculateSquareOutput> Execute(CalculateSquareInput input)
    {
        var squared = input.Number * input.Number;
        // Simulate long-running operation
        await Task.Delay(1000);
        return new CalculateSquareOutput { Squared = squared };
    }
}
```

### 2. Define a Job

```csharp
public class CalculateSumOfSquaresJob : Job<CalculateSumOfSquaresInput, CalculateSumOfSquaresOutput>
{
    private readonly Operation<CalculateSquareInput, CalculateSquareOutput> _calculateSquare;

    public CalculateSumOfSquaresJob(Operation<CalculateSquareInput, CalculateSquareOutput> calculateSquare)
    {
        _calculateSquare = calculateSquare;
    }

    public async Task<CalculateSumOfSquaresOutput> Execute(CalculateSumOfSquaresInput input)
    {
        // Execute operations in parallel
        var operations = input.Numbers.Select(number => 
            _calculateSquare.Execute(new CalculateSquareInput { Number = number }));
        
        var results = await Task.WhenAll(operations);
        var sum = results.Sum(result => result.Squared);
        
        return new CalculateSumOfSquaresOutput { Sum = sum };
    }
}
```

### 3. Configure Services

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.UseFlow()
        .UseJob<CalculateSumOfSquaresJob, CalculateSumOfSquaresInput, CalculateSumOfSquaresOutput>()
        .UseOperation<CalculateSquareOperation, CalculateSquareInput, CalculateSquareOutput>();
}
```

### 4. Execute Jobs

```csharp
// Create and start a job
var job = serviceProvider.GetRequiredService<Job<CalculateSumOfSquaresInput, CalculateSumOfSquaresOutput>>();
var jobId = await job.Create(new CalculateSumOfSquaresInput { Numbers = [1, 2, 3, 4, 5] });

// Check job status
var status = await job.GetStatus(jobId);
Console.WriteLine($"Progress: {status.CompletedSteps}/{status.TotalSteps}");

// Get result when complete
if (status.IsCompleted)
{
    var result = await job.GetResult(jobId);
    Console.WriteLine($"Sum of squares: {result.Sum}");
}
```

## Working with External Inputs

Flow supports workflows that require external data input:

```csharp
public class UpdateEmailJob : Job<UpdateEmailJobInput, UpdateEmailJobOutput>
{
    private readonly ExternalInput<ConfirmExternalInputSeed, ConfirmExternalInput> _externalInput;
    private readonly Operation<UpdateEmailOperationInput, UpdateEmailOperationOutput> _updateEmail;

    public UpdateEmailJob(
        ExternalInput<ConfirmExternalInputSeed, ConfirmExternalInput> externalInput,
        Operation<UpdateEmailOperationInput, UpdateEmailOperationOutput> updateEmail)
    {
        _externalInput = externalInput;
        _updateEmail = updateEmail;
    }

    public async Task<UpdateEmailJobOutput> Execute(UpdateEmailJobInput input)
    {
        // Create external input with seed data
        var confirmationInput = _externalInput.Create(new ConfirmExternalInputSeed
        {
            UserId = input.UserId,
            NewEmail = input.NewEmail
        });

        // This will pause execution until external data is provided
        var providedInput = await confirmationInput.Get();
        
        if (providedInput.Confirm)
        {
            await _updateEmail.Execute(new UpdateEmailOperationInput 
            { 
                UserId = input.UserId, 
                NewEmail = input.NewEmail 
            });
        }

        return new UpdateEmailJobOutput();
    }
}

// Providing external data from an API endpoint
var externalInput = serviceProvider.GetRequiredService<ExternalInput>();
await externalInput.Provide("job-id:input-id", new ConfirmExternalInput { Confirm = true });
```

## Deployment Options

### Local Development

```csharp
services.UseFlow()
    .UseLocalFlow(); // Adds local message polling
```

### AWS Lambda

```csharp
services.UseFlow()
    .UseLambdaFlow(); // Adds Lambda-specific message handling
```

## How It Works

1. **Job Creation**: When a job is created, it's stored in partitioned storage and a message is queued
2. **Job Execution**: The message handler picks up the job and begins execution
3. **Operation Scheduling**: When operations are encountered, they're scheduled as separate messages
4. **Operation Execution**: Each operation runs independently and stores its result
5. **Job Completion**: Once all operations complete, the job resumes and produces its final result
6. **External Input Handling**: External input points pause execution until data is provided

## Storage Requirements

Flow requires implementations of:
- `Staticsoft.PartitionedStorage.Abstractions` for state persistence
- `Staticsoft.Messages.Abstractions` for message queuing
- `Staticsoft.Serialization.Abstractions` for data serialization

## Testing

The project includes comprehensive tests demonstrating:
- Complete job execution with multiple operations
- External input-based workflows
- Progress tracking
- Error handling and retries

Run tests with:
```bash
dotnet test Flow.Tests
```

## Example Applications

The repository includes a complete test server (`TestServer`) that demonstrates:
- REST API endpoints for job management
- Complex workflows with parallel operations
- External input-based workflows
- Both local and Lambda deployment configurations

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

This is a Staticsoft project. Please refer to the project's contribution guidelines for more information.

## Dependencies

- .NET 8.0
- Microsoft.AspNetCore.App
- Polly (for retry policies)
- Various Staticsoft abstractions for storage, messaging, and serialization

## Repository

- **GitHub**: https://github.com/Staticsoft/Flow
- **Package**: Staticsoft.Flow (NuGet)
