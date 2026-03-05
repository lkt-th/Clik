# Clik

Clik is a .NET library (solution) in this repository. It includes a `Clik.UnitOfWork` project that provides implementations for common data-access patterns such as Unit of Work and Repository.

## Requirements

- .NET 10 SDK

## Project layout

- `Clik.UnitOfWork/` - Library project containing Unit of Work and Repository implementations (interfaces and concrete classes).
- Other projects (if present) will appear in the solution and can be explored in the repository root.

## Usage

Reference the `Clik.UnitOfWork` project or its NuGet package from your application. Look at interfaces such as `IRepository` in the `Clik.UnitOfWork` project to understand available operations.

Install from NuGet (when published)

dotnet:

```
dotnet add package Clik.UnitOfWork --version <version>
```

Package Manager Console (Visual Studio):

```
Install-Package Clik.UnitOfWork -Version <version>
```

Replace `<version>` with the released package version.

## Contributing

Contributions, bug reports and pull requests are welcome. Please follow the repository's existing coding style when submitting changes.

## License

Check the repository for a `LICENSE` file for license details. If none is provided, contact the repository owner.

## Contact

Repository: `https://github.com/lkt-th/Clik`
