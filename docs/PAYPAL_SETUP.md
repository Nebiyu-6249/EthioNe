# PayPal Checkout for Ad Packages — setup

> Lets advertisers pay for an Ad Package online with **PayPal** and have the ad activated
> automatically, in addition to the existing manual bank-transfer flow (which is unchanged).
> The whole feature is **inert until you set `PayPal:ClientId` + `PayPal:Secret`** in the
> server Web.config — with those blank, no button appears and no PayPal code runs. Start in
> **sandbox**, verify a full payment, then switch to **live**.

## How it works
- `Application/Payments/PayPalService.cs` is a tiny, self-contained client for the PayPal
  **Orders v2** REST API (no SDK / NuGet dependency — just `HttpWebRequest` + JSON). It reads
  everything from Web.config `<appSettings>` (`PayPal:*`) and exposes `IsConfigured`,
  `CreateOrder(...)` and `CaptureOrder(...)`.
- On the payment page (`Payment/Create`), a **"Pay with PayPal"** button appears **only when
  `IsConfigured` is true**. It sits alongside the normal bank-transfer form as an alternative.
- Flow (all server-side; the client is never trusted with the amount or the outcome):
  1. `PaymentController.PayPalCheckout` recomputes the price with the **same formula** as the
     manual flow (`month × AdCostPerMonth × PriceRate`, minus the package `Discount %`), creates
     a PayPal order for that amount, saves a **Pending** `Payment` keyed by the PayPal order id
     (stored in `Reference`), marks the ad `Pending`, and redirects the payer to PayPal.
  2. The payer approves on PayPal and is returned to `PaymentController.PayPalReturn?token=<orderId>`.
  3. The server **captures** the order and verifies the status is `COMPLETED` before granting
     anything. On success it activates the ad exactly like an admin confirmation does
     (`DateStart`/`DateEnd`, `AdStatus.Active`, invoice number, `PaymentStatus.Confirmed`) and
     sends the confirmation email (best-effort — a mail failure never blocks activation).
  4. Cancel returns to `PaymentController.PayPalCancel`, which drops the abandoned Pending
     payment so the ad reverts to `Partial` and can be paid again cleanly.
- It is **idempotent**: a return for an already-confirmed order shows success without capturing
  again, so a browser refresh can't double-charge or double-activate.

## Currency — important
Ad prices are in **ETB**, which PayPal does **not** settle in. So the charge is made in a
supported currency (default **USD**) controlled by two settings:
- `PayPal:Currency` — the currency PayPal is charged in (default `USD`).
- `PayPal:AmountFactor` — multiplier applied to the ETB package price to get that amount.
  Default `1.0` (no conversion — i.e. treat the package number as already being in
  `PayPal:Currency`). To charge a USD approximation of the ETB price, set e.g. `0.018`
  (≈ current ETB→USD). Pick a value you're comfortable with; there is no live FX lookup.

The invoice/records keep the original ETB figures; the actual captured amount + currency +
PayPal capture id are appended to the payment's `Comment` for reconciliation.

## Where credentials go (server Web.config `<appSettings>`)
Web.config is excluded from FTP, so these live only on the server and are never committed.
Add to `h:\root\home\nebiyu6249-001\www\site1\Web.config`:
```xml
<add key="PayPal:ClientId"     value="YOUR_PAYPAL_CLIENT_ID" />
<add key="PayPal:Secret"       value="YOUR_PAYPAL_SECRET" />
<add key="PayPal:Environment"  value="sandbox" />   <!-- sandbox | live -->
<add key="PayPal:Currency"     value="USD" />
<add key="PayPal:AmountFactor" value="1.0" />
```
(The same keys appear commented in the repo Web.config for reference.)

## Get credentials
1. https://developer.paypal.com → log in → **Apps & Credentials**.
2. Toggle **Sandbox** (top) → **Create App** → copy the **Client ID** and **Secret** into
   `PayPal:ClientId` / `PayPal:Secret` with `PayPal:Environment=sandbox`.
3. Test with a sandbox buyer account (Testing tools → **Sandbox Accounts**).
4. When satisfied, switch to the **Live** tab, create a live app, replace the two credentials,
   and set `PayPal:Environment=live`.

You do **not** register redirect URIs with PayPal for Orders v2 — the return/cancel URLs are
sent per-order by the server (`Url.Action(...)`), so the temp URL and the live domain both work.

## Turn it on
1. Run `database/AddPayPalKeys.sql` once (adds the 6 localization keys, en + am — idempotent).
2. Put the sandbox credentials in the server Web.config (above).
3. Recycle the app pool (touch `/site1/Web.config`).
4. Create an ad → choose a package → the **Pay with PayPal** button now shows; pay with a
   sandbox buyer account and confirm the ad flips to Active and an invoice number is assigned.
5. Repeat with live credentials + `Environment=live` when ready to accept real money.

## Notes / limits
- All PayPal HTTP calls have short timeouts (15–20 s); a network/API failure surfaces a
  localized "could not be completed" message and never leaves the ad half-activated.
- Rare edge case: if PayPal captures the money but the DB commit then fails, the payment stays
  Pending and is logged (`LoggingService.Error`) — reconcile from the PayPal dashboard and
  confirm the payment manually in the admin Payments list.
