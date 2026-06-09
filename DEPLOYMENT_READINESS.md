# EthioNegarit — Deployment Readiness

## Can we deploy today? — the honest answer

**The source is now deployment-ready. Going *live* today is possible but tight, and depends entirely on steps that must run on a Windows machine — not on anything that's left in the code.** I've made every critical source-level fix from the consultancy report. What remains is environment work (build, database, TLS, smoke test) that I can't do from here. If you have a Windows build machine and a server/host ready, a careful same-day launch is realistic. If any of those aren't set up yet, give it a day so you're not debugging a fresh .NET 4.8 build under launch pressure.

The one thing I will not pretend: I could not **compile** this, because .NET Framework builds only on Windows with MSBuild. Every edit below is correct and standard, but you must rebuild and smoke-test on Windows before flipping DNS to your domain.

---

## What I changed (done)

### 1. Runtime upgrade — .NET Framework 4.5.2 → 4.8 (your "must change the outdated runtime" item)
- Retargeted all **8 project files** (`*.csproj`) from `v4.5.2` to `v4.8`.
- Updated `Web.config` `httpRuntime targetFramework` and `compilation targetFramework` to `4.8`.
- This is an in-place, low-risk upgrade: 4.8 runs your existing 4.5.2 assemblies, so package upgrades are not required to build. The only requirement is that the **build machine has the .NET Framework 4.8 Developer Pack** installed (see below).

### 2. Production safety (launch blockers from the report)
- `customErrors` `Off` → **`RemoteOnly`** — stops leaking stack traces to visitors.
- `compilation debug="true"` → **`debug="false"`** — proper production performance/security.
- Forms-auth `timeout` `432000` min (300 days) → **`10080`** (7 days), with sliding expiration.

### 3. Authentication / cookie hardening
- Added `requireSSL="true"` to the forms-auth cookie.
- Added `<httpCookies httpOnlyCookies="true" requireSSL="true" />`.
- Added a **commented `<machineKey>`** block with instructions — set this for production so logins/anti-forgery survive app-pool recycles and multiple servers.

### 4. Password policy
- `minRequiredPasswordLength` `6` → **`8`**. (Affects new passwords only; existing users are unaffected.)

### 5. Fixed a latent runtime bug I found while editing
- The membership/role/profile providers reference `connectionStringName="EthioNegarit"`, but the only connection string defined was named `EthioNegaritContext`. That mismatch can throw at runtime. I added an **`EthioNegarit`** connection string alongside `EthioNegaritContext` (both must point at the same database).

### 6. Connection strings → production placeholders
- Replaced the LocalDB connection string with **clearly-marked production placeholders** (`YOUR_SQL_SERVER`, `YOUR_DB_USER`, `YOUR_DB_PASSWORD`). Both connection strings now use `Encrypt=True`.
- **Do not commit real credentials.** See "Secrets" below.

### 7. Security response headers (`system.webServer`)
- Removed `X-Powered-By`; added `X-Content-Type-Options: nosniff`, `X-Frame-Options: SAMEORIGIN`, `Referrer-Policy`, and `Strict-Transport-Security` (HSTS — active once you're on HTTPS).

### 8. Repo hygiene
- Added a proper **`.gitignore`** (excludes `bin/`, `obj/`, `packages/`, `*.exe`, `*.mdf/*.ldf`, secret config files).
- See the "remove committed binaries" step below — the `.gitignore` stops *future* commits, but already-tracked files must be untracked once.

---

## What still needs YOU (Windows side) — in order

1. **Install build prerequisites (one time):** Visual Studio 2019/2022 (or Build Tools) **with the .NET Framework 4.8 Developer Pack / targeting pack**. Without the 4.8 pack, the retargeted projects won't build.

2. **Drop in these files** over your working tree (they preserve relative paths): the 8 `*.csproj`, `EthioNegarit.WebUI/Web.config`, and `.gitignore` at the solution root.

3. **Restore + rebuild:** `nuget restore`, then build the solution in **Release**. Fix any build errors (expected to be minimal for a 4.5.2→4.8 retarget). **This is the step I could not do for you — do not skip it.**

4. **Remove committed binaries from git (one time):**
   ```
   git rm -r --cached bin obj packages
   git rm --cached ngrok.exe
   git commit -m "Stop tracking build output, packages, and binaries"
   ```
   Then delete `ngrok.exe` from the working folder — it must not ship.

5. **Stand up the production database:** create the SQL Server DB, run the app's migrations/initializer against it, and put the real credentials into the two connection strings (`EthioNegaritContext` and `EthioNegarit`). Prefer keeping them out of source (next item).

6. **Move secrets out of source control:** either use a `Web.Release.config` transform to inject the production connection strings at publish time, or split them into a separate file referenced via `configSource="connectionStrings.config"` and keep that file un-tracked (already in `.gitignore`).

7. **Set the `machineKey`:** generate keys (IIS Manager → Machine Key → Generate, untick "generate at runtime") and uncomment the `<machineKey>` block in `Web.config`.

8. **TLS / HTTPS:** install a certificate for your domain (Let's Encrypt / win-acme, or terminate TLS at Cloudflare). The cookies are now `requireSSL="true"`, so **login will not work over plain HTTP** — HTTPS must be live. Add an HTTP→HTTPS redirect at IIS or Cloudflare (not added in config, since the IIS URL Rewrite module isn't guaranteed to be installed and a missing module causes a 500).

9. **Smoke test before DNS cutover:** register a user, log in, post a question/answer, vote, mark a solution, upload an image, open the chat room, switch to Amharic, and submit a payment record. Confirm error pages are friendly (no stack traces) and the site is HTTPS-only.

---

## One item I deliberately did NOT auto-change: `requestValidationMode="2.0"`

The report flagged this as an XSS surface. I left it at `2.0` **on purpose**, because flipping it to `4.x` globally will very likely break your rich-text (TinyMCE) post submission on launch day — `2.0` mode is almost certainly what lets HTML content be posted.

The correct, safe fix (do this *after* launch, with testing):
1. Set `requestValidationMode` back to the framework default.
2. Add `[AllowHtml]` only to the specific model fields that legitimately carry HTML (post/topic body).
3. Run that HTML through a server-side sanitizer (you already ship AntiXSS) before saving, so stored content can't carry script.

Until then, make sure every place that renders user-submitted HTML is sanitized — the client-side chat fix already escapes message text.

---

## Optional (not blocking)
- `packages.config` entries still say `targetFramework="net452"`. Harmless for the build (assemblies are forward-compatible); you can refresh them later via *Update-Package -Reinstall* if you want clean metadata.
- Consider bumping a few NuGet packages to their latest 4.8-compatible versions for security patches — do this incrementally, testing after each.
