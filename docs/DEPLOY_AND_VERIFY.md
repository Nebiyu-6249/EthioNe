# Deploy / verify / recycle — Ethiopical

## Part 0 — state audit (done)
`main` contains every Task 0–5 fix (verified: the Topic.NewsAndEvent null guard, the
845-key en-GB/am-ET CSVs, the chat list + room rewrites, the Google redirect fix). The
CI run that deployed `main` (run #21, the PR #6 merge) was **green**. So the code and the
DB SQL you already ran ARE deployed.

## The #1 reason "nothing changed": the app pool was not truly recycled
The DB changes (rebrand) and the new DLL are in place, but the running app is still
serving cached/stale state:
- **Settings** (brand / ForumName) are cached ~1 day → re-read only after an app-pool recycle.
- **Localization** strings are cached → same.
- A **freshly deployed DLL** (the login fix, chat redesign) is only loaded after a real
  app-domain restart.

### Recycle checklist (do this after EVERY deploy or DB settings/localization change)
1. FTP to the server and open `/site1/Web.config`.
2. Add a blank line (or a space) and save — this "touch" forces a full app-domain restart.
   (The SmarterASP "Pool" button alone sometimes does not fully recycle a wedged worker.)
3. Wait ~90 seconds.
4. Open the site in a **new incognito window** (avoids browser + session cache) and hard-refresh.

After this, the brand should read **Ethiopical** (the rebrand SQL already succeeded in the
DB), the login fix and chat redesign should be live, and localized strings appear once
imported (below).

---

## Part 1 — import the 845 localization keys (two ways)

First confirm both languages and note their Ids:
```sql
SELECT Id, Name, LanguageCulture FROM [Language];
```
You should see `en-GB` (English (United Kingdom)). `am-ET` (Amharic) may or may not exist
yet — the admin import below will create it if missing.

### Option A (recommended) — admin Import/Export, no size limit
1. Log in as admin, go to **Admin → Import / Export → Languages**
   (`/admin/ImportExport/Languages`).
2. In **Import a Language**: pick **English (United Kingdom)** from the dropdown, choose the
   file `en-GB.csv`, click **Import**.
3. Repeat: pick **Amharic (Ethiopia)**, choose `am-ET.csv`, **Import**.
4. Recycle the app pool (checklist above).

The CSV files are in the repo at `EthioNegarit.WebUI/Installer/en-GB.csv` and `am-ET.csv`
(also on the server at `/site1/Installer/`). The importer is an upsert: it reuses existing
keys (never creates duplicates) and updates/creates each language's value.

### Option B (fallback) — split SQL in Run Query
If the admin screen isn't usable, run these in MSSQL Manager → Run Query, one at a time
(each is small enough for the web form that rejected the single big script with ASP 0104):
```
database/SyncLocalization_part01.sql … part09.sql
```
Each part is self-contained and idempotent (insert-if-not-exists on key Name + per-language
value; never duplicates keys). Order doesn't matter. Recycle the pool when done.

---

## Part 2 — what each fix needs to become visible
- **Brand → "Ethiopical"**: DB already updated by `RebrandToEthiopical.sql`; shows after a recycle.
- **Login NullReferenceException** (entering an unknown username 500'd at `MembersController.LogOn`):
  fixed in code — an unknown/invalid login now shows the friendly "password incorrect" message.
  Live after the deploy + recycle.
- **Change Language**: works once both languages' strings are imported (Part 1) and the pool
  is recycled.

## Cross-cutting
- Once localization is in and verified, set `<compilation … debug="false" />` in the server
  `Web.config` (the SquishIt JS fix is in place) and recycle. Confirm styles/scripts still load.
- Keep the FTP exclude root-only (`Web.config`, `web.config`, `App_Data/forum.config`); never
  `**/Web.config`. Keep `dangerous-clean-slate: false`.
- Security still open: rotate the exposed DB password (update both connection strings in the
  server Web.config) and the Google OAuth secret. Never commit secrets.
