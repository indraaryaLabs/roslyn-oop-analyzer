# MOOD Metrics Analyzer

## 📌 Deskripsi Proyek
**MOOD Metrics Analyzer** adalah aplikasi desktop berbasis Windows Forms (C#) yang berfungsi untuk melakukan analisis statis pada kode sumber C#. Aplikasi ini menggunakan **Microsoft Roslyn Compiler API** untuk mengekstrak *Syntax Tree* dan *Semantic Model*, kemudian menghitung metrik kualitas desain perangkat lunak berorientasi objek berdasarkan standar **MOOD (Metrics for Object-Oriented Design)**.

Proyek ini bertujuan untuk membantu *developer* mengevaluasi kualitas arsitektur OOP pada kode mereka melalui angka metrik yang terukur.

## 📊 Metrik yang Dihitung
Aplikasi ini memindai seluruh file `.cs` di dalam folder target dan menghitung 6 metrik utama:
1. **MHF (Method Hiding Factor):** Mengukur tingkat enkapsulasi metode.
2. **AHF (Attribute Hiding Factor):** Mengukur tingkat enkapsulasi atribut.
3. **MIF (Method Inheritance Factor):** Mengukur sejauh mana pewarisan metode digunakan.
4. **AIF (Attribute Inheritance Factor):** Mengukur sejauh mana pewarisan atribut digunakan.
5. **CF (Coupling Factor):** Mengukur tingkat ketergantungan (kopling) antar kelas non-pewarisan.
6. **PF (Polymorphism Factor):** Mengukur derajat *overriding* metode dan potensi polimorfisme.

## 🛠️ Teknologi yang Digunakan
- **Bahasa Pemrograman:** C#
- **Framework:** .NET 6.0 (Windows Forms)
- **Static Code Analysis:** `Microsoft.CodeAnalysis.CSharp` (Roslyn API)
- **Asynchronous Programming:** `async`/`await` untuk menjaga responsivitas UI selama proses analisis.

## 🚀 Cara Menjalankan Aplikasi

**Prasyarat:**
- Visual Studio 2022 (dengan *workload* .NET desktop development)
- .NET 6.0 SDK

**Langkah-langkah:**
1. *Clone repository* ini:
   ```bash
   git clone [https://github.com/indraarya77/roslyn-oop-analyzer.git](https://github.com/indraarya77/roslyn-oop-analyzer.git)