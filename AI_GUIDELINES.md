# BASTION Cybersecurity Suite — AI Agent Guidelines
This file outlines critical architectural constraints, code boundaries, and system rules. Any AI agent modifying this codebase must adhere strictly to these principles to maintain integrity and prevent regression.

---

## 1. Local Security & API Secret Isolation (CRITICAL)
* **Rule:** **NEVER** write or commit active API keys (Groq, AbuseIPDB, Google Safe Browsing, URLScan) to the tracked `appsettings.json` file. Doing so triggers GitHub Push Protection and blocks commits.
* **Standard:** Keep all live API keys inside the gitignored local file: `appsettings.Development.json`. BASTION's configuration manager automatically overrides placeholders during local runs using this file.
* **Key Placeholders:** Leave placeholders (e.g. `YOUR_GROQ_API_KEY_HERE`) exactly as they are in `appsettings.json`.

---

## 2. Real-time Administrative Eviction & Lockout Loop
* **Rule:** Do not decouple the database state transitions from real-time events.
* **Standard:** 
  * Modifying session states (Revocation or Ban Toggles) must always trigger:
    ```csharp
    AuthService.TriggerSessionRevocation(email, isBan: true/false);
    ```
  * Active layouts (`MainLayout.razor`) subscribe to these static events to execute real-time eviction across all open browser channels (e.g., standard tabs, incognito windows, or distinct browsers).
  * The GateKeeper login portal expects `?reason=banned` or `?reason=revoked` to alert the user with the correct custom error message.

---

## 3. Database-Linked OAuth & SSO Integrations
* **Rule:** Do not disable or mock-out the Google and Microsoft SSO login actions with simple static alerts.
* **Standard:** Both SSO buttons in `LoginForm.razor` must remain active and functional. They simulate the secure handshake, register the corresponding accounts in the SQLite database automatically on first click, register active session details, and sign in standard profiles seamlessly.

---

## 4. Markdown Formatting & Parsing Rules (Support Portal)
* **Rule:** BASTION's Support portal features a zero-dependency, line-by-line custom C# Markdown-to-HTML parser designed to avoid rendering raw markdown syntax in the support chat blocks.
* **Standard:** 
  * Do not output raw hashtags (`#`, `##`, `###`) for headings or raw asterisks for bullet points inside support agent responses.
  * The custom parser in `Support.razor` automatically maps stripped lines to corresponding premium-designed CSS-variable CSS blocks. Keep response formatting clean and structured.

---

## 5. UI Theme & Accent Controls (CSS Standards)
* **Rule:** BASTION utilizes a responsive theme layout system controlled via `ThemeService.cs` and applied in the DOM via `bastionTheme.apply`.
* **Standard:** Accent modifications must follow the strict design guidelines:
  * Global custom color wheel accents are exclusively restricted to **Light Mode** to maintain high-end readability.
  * In **Dark Mode**, the dashboard must default to its premium, sleek glassmorphic cyber-theme layout.

---

## 6. Decoupled Modular Architecture
* **Rule:** Maintain BASTION's strict zero-coupling modular system.
* **Standard:** Dashboard features (CyberShield, LexGuard, FinShield, HackerLab, Settings, Support) must remain completely decoupled. Do not share raw state containers or couple databases directly across components. Use services and DI containers registered in `Program.cs` for clean, modular communication.
