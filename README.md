# roslyn-oop-analyzer

A Windows desktop tool that statically analyzes C# codebases with the Roslyn compiler API and reports **MOOD metrics** (Metrics for Object-Oriented Design) per class and for the whole system.

## ✨ Features

- **Six standard MOOD metrics**, computed with Roslyn syntax trees and semantic models — not regex heuristics:

  | Metric | Measures |
  | --- | --- |
  | **MHF** — Method Hiding Factor | Encapsulation of methods (private / effectively hidden) |
  | **AHF** — Attribute Hiding Factor | Encapsulation of attributes (fields and properties) |
  | **MIF** — Method Inheritance Factor | Share of methods actually inherited and usable |
  | **AIF** — Attribute Inheritance Factor | Share of attributes actually inherited and usable |
  | **CF** — Coupling Factor | Non-inheritance coupling between classes |
  | **PF** — Polymorphism Factor | Degree of method overriding relative to polymorphic potential |

- **Folder-based scanning** — pick any folder and every `.cs` file (including subfolders) is parsed into a single Roslyn compilation; `*.Designer.cs` files, the entry-point `Program` class, and extension-method-only static classes are excluded automatically
- **Two-level reporting** — per-class breakdowns of declared/hidden/inherited members and coupled classes, plus a system-level summary that shows each metric's formula, worked calculation, and final percentage
- **Polymorphism-aware** — descendant counts per class are resolved from the type hierarchy before PF is computed
- **Export** — save the full metrics summary as a `.txt` report
- **Responsive analysis** — parsing and metric computation run asynchronously with a progress bar, keeping the UI interactive on large codebases
- **Debug panel** — optional detailed internal calculations per class for verifying results
- **Polished desktop shell** — MaterialSkin-based UI with fullscreen toggle, resizable layout, and built-in help

## 🏗️ Project structure

```
.
├── MOOD Metrics.sln
└── MOOD Metrics/
    ├── Form1.cs             # UI logic + RoslynMoodAnalyzer (all metric computation)
    ├── Form1.Designer.cs    # WinForms designer layout
    ├── Form1.resx
    ├── Program.cs           # application entry point
    └── MOOD Metrics.csproj  # .NET 6.0 Windows Forms project
```

## 🛠️ Tech stack

| Layer | Technology |
| --- | --- |
| Language | C# |
| UI | Windows Forms on .NET 6.0, MaterialSkin.2 |
| Static analysis | Microsoft.CodeAnalysis.CSharp + Workspaces (Roslyn) 4.x |
| Concurrency | async/await + Task.Run for background analysis |

## 🚀 Getting started

**Prerequisites:**

- Visual Studio 2022 with the **.NET desktop development** workload
- .NET 6.0 SDK (Windows)

```bash
git clone https://github.com/indraaryaLabs/roslyn-oop-analyzer.git
```

1. Open **`MOOD Metrics.sln`** in Visual Studio.
2. Press **F5** (or Build → Run) to launch the app.
3. Click **Browse** and select a folder containing C# source files.
4. Click **Analyze** — the left pane lists the detected files, the center pane shows per-class metric details, and the right pane shows the system-level MOOD summary.
5. Optionally enable **Debug** to see intermediate calculations, and use **Export** to save the summary as a text file.

## 📄 License

Released under the [MIT License](LICENSE).
