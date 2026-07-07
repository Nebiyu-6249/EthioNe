namespace EthioNegarit.WebUI.Controllers
{
    using System;
    using System.Linq;
    using Core.Interfaces.Services;
    using Core.Interfaces.UnitOfWork;
    using Core.Constants;
    using System.Web.Mvc;
    using Areas.Admin.ViewModels;
    using ViewModels;
    using Core.Domain.Entities;
    using Utilities;
    using Core.Domain.Enums;
    using System.Text;
    using Application;
    using Application.Payments;
    using System.Data.SqlTypes;
    public partial class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IPackageService _packageService;
        private readonly IAdService _adService;
        private readonly IEmailService _emailService;
        private readonly ILocalizationService _localizationService;
        public PaymentController(ILogAuditService logAuditService, ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, IPackageService packageService, IAdService adService, IPaymentService paymentService, IPrivateMessageService privateMessageService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, IEmailService emailService, ICacheService cacheService)
            : base(logAuditService, loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService, cacheService)
        {
            _paymentService = paymentService;
            _packageService = packageService;
            _adService = adService;
            _emailService = emailService;
            _localizationService = localizationService;
        }
        // GET: Payment
        [Authorize(Roles = AppConstants.AdminRoleName)]
        public ActionResult Index(int? p)
        {
            var viewQuerystring = Request.QueryString[AppConstants.PaymentViewOrder];
            var view = !string.IsNullOrEmpty(viewQuerystring) ? EnumUtils.ReturnEnumValueFromString<PaymentViewOrder>(viewQuerystring) : PaymentViewOrder.All;
            var settings = SettingsService.GetSettings();
            var pageIndex = p ?? 1;
            var viewModel = new PaymentsListViewModel();
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var payments = _paymentService.GetPayments(pageIndex, settings.TopicsPerPage, SiteConstants.Instance.AdminListPageSize, view);
                viewModel.Payments = payments;
                viewModel.PageIndex = pageIndex;
                viewModel.TotalCount = payments.TotalCount;
                viewModel.TotalPages = payments.TotalPages;
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_payments", viewModel);
            }
            else
            {
                return View(viewModel);
            }
        }
        [Authorize]
        public ActionResult List(int? p)
        {
            var settings = SettingsService.GetSettings();
            var pageIndex = p ?? 1;
            var viewModel = new PaymentsListViewModel();
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                var payments = _paymentService.GetMemberPayments(pageIndex, settings.TopicsPerPage, SiteConstants.Instance.AdminListPageSize, loggedOnUser);
                viewModel.Payments = payments;
                viewModel.PageIndex = pageIndex;
                viewModel.TotalCount = payments.TotalCount;
                viewModel.TotalPages = payments.TotalPages;
            }
            return View(viewModel);
        }
        [Authorize]
        public ActionResult Create(Guid packageId, Guid advertId, int month)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                var advert = _adService.Get(advertId);
                var package = _packageService.GetPackage(packageId);
                var grossPrice = month * settings.AdCostPerMonth * package.PriceRate;
                var discount = grossPrice * package.Discount / 100;
                var totalPrice = grossPrice - discount;
                var viewModel = new CreateEditPaymentViewModel
                {
                    AdPackage = package,
                    PackageId = package.Id,
                    User = loggedOnUser,
                    Advertisement = advert,
                    AdvertId = advert.Id,
                    Month = month,
                    GrossPrice = (int)grossPrice,
                    Discount = (int)discount,
                    TotalPrice = (double)totalPrice,
                    Amount = (double)totalPrice
                };
                // Show the "Pay with PayPal" button only when PayPal credentials are configured on the server.
                ViewBag.PayPalEnabled = new PayPalService().IsConfigured;
                return View(viewModel);
            }
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateEditPaymentViewModel viewModel)
        {
            Payment payment;
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    var package = _packageService.GetPackage(viewModel.PackageId);
                    var advert = _adService.Get(viewModel.AdvertId);
                    //Check Payment not Used before
                    var payments = _paymentService.GetPaymentByReference(viewModel.Reference);
                    /* if (payments.Count > 0)*/
                    if (payments.Any())
                    {
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Payment.Error.InvalidReference"));
                        return View(viewModel);
                    }
                    //Check Transfered Amount is valid
                    if (viewModel.Amount < viewModel.TotalPrice)
                    {
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Payment.Error.InvalidAmount"));
                        return View(viewModel);
                    }
                    // Create the Payment model
                    payment = new Payment();
                    payment.InvoiceNo = 0;
                    payment.Reference = viewModel.Reference;
                    payment.Month = viewModel.Month;
                    payment.TotalPrice = viewModel.TotalPrice;
                    payment.Discount = viewModel.Discount;
                    payment.Amount = viewModel.Amount;
                    payment.DateTransfer = viewModel.DateTransfer;
                    payment.Bank = viewModel.Bank;
                    payment.AccountNo = viewModel.AccountNo;
                    payment.DateConfirmed = (DateTime)SqlDateTime.MinValue;
                    payment.ConfirmedBy = null;
                    payment.User = loggedOnUser;
                    payment.PaymentStatus = PaymentStatus.Pending;
                    payment.Package = package;
                    payment.Advertisement = advert;
                    payment.Comment = viewModel.Comment;
                    payment.DateSubmitted = DateTime.UtcNow;
                    payment = _paymentService.Add(payment);
                    advert.AdStatus = AdStatus.Pending;
                    try
                    {
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Payment.Submitted"),
                            MessageType = GenericMessages.info
                        };
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
                return RedirectToAction("List", "Payment");
            }
            return View(viewModel);
        }

        // ---------------------------------------------------------------------
        // PayPal Checkout (Orders v2). The whole flow is inert unless PayPal
        // credentials are set on the server (Web.config appSettings PayPal:*).
        // Amounts are verified server-side on capture before the ad is granted.
        // ---------------------------------------------------------------------

        // Step 1: create a PayPal order for the chosen package/duration, persist a
        // Pending payment keyed by the PayPal order id, then redirect to PayPal.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult PayPalCheckout(Guid packageId, Guid advertId, int month)
        {
            var payPal = new PayPalService();
            if (!payPal.IsConfigured)
            {
                ShowMessage(new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("PayPal.NotConfigured"),
                    MessageType = GenericMessages.warning
                });
                return RedirectToAction("Create", new { packageId, advertId, month });
            }

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                var advert = _adService.Get(advertId);
                var package = _packageService.GetPackage(packageId);
                if (advert == null || package == null)
                {
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }

                // Same price formula as the manual (bank transfer) flow.
                var grossPrice = month * settings.AdCostPerMonth * package.PriceRate;
                var discount = grossPrice * package.Discount / 100;
                var totalPrice = grossPrice - discount;
                var charge = payPal.ToChargeAmount((double)totalPrice);

                var returnUrl = Url.Action("PayPalReturn", "Payment", null, Request.Url.Scheme);
                var cancelUrl = Url.Action("PayPalCancel", "Payment", null, Request.Url.Scheme);
                var order = payPal.CreateOrder(charge, advert.Id.ToString(), returnUrl, cancelUrl);
                if (!order.Success)
                {
                    LoggingService.Error("PayPal create-order failed: " + order.Error);
                    ShowMessage(new GenericMessageViewModel
                    {
                        Message = LocalizationService.GetResourceString("PayPal.Failed"),
                        MessageType = GenericMessages.danger
                    });
                    return RedirectToAction("Create", new { packageId, advertId, month });
                }

                // Persist a Pending payment keyed by the PayPal order id (Reference)
                // so the capture on return can be reconciled server-side.
                var payment = new Payment
                {
                    InvoiceNo = 0,
                    Reference = order.OrderId,
                    Month = month,
                    TotalPrice = (double)totalPrice,
                    Discount = (double)discount,
                    Amount = (double)totalPrice,
                    DateTransfer = DateTime.UtcNow,
                    PaymentMode = "PayPal",
                    AccountNo = order.OrderId,
                    DateConfirmed = (DateTime)SqlDateTime.MinValue,
                    ConfirmedBy = null,
                    User = loggedOnUser,
                    PaymentStatus = PaymentStatus.Pending,
                    Package = package,
                    Advertisement = advert,
                    Comment = string.Format("PayPal order {0} for {1} {2}", order.OrderId, charge.ToString("0.00"), payPal.Currency),
                    DateSubmitted = DateTime.UtcNow
                };
                _paymentService.Add(payment);
                advert.AdStatus = AdStatus.Pending;

                try
                {
                    unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    ShowMessage(new GenericMessageViewModel
                    {
                        Message = LocalizationService.GetResourceString("PayPal.Failed"),
                        MessageType = GenericMessages.danger
                    });
                    return RedirectToAction("Create", new { packageId, advertId, month });
                }

                return Redirect(order.ApproveUrl);
            }
        }

        // Step 2: PayPal redirects the payer back here (?token=<orderId>&PayerID=...).
        // Capture the order, verify it COMPLETED server-side, then activate the ad.
        [Authorize]
        public ActionResult PayPalReturn(string token, string PayerID)
        {
            if (string.IsNullOrEmpty(token))
            {
                ShowMessage(new GenericMessageViewModel
                {
                    Message = LocalizationService.GetResourceString("PayPal.Failed"),
                    MessageType = GenericMessages.danger
                });
                return RedirectToAction("List", "Payment");
            }

            var payPal = new PayPalService();
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = SettingsService.GetSettings();
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                var payment = _paymentService.GetPaymentByReference(token).FirstOrDefault();
                if (payment == null)
                {
                    ShowMessage(new GenericMessageViewModel
                    {
                        Message = LocalizationService.GetResourceString("PayPal.Failed"),
                        MessageType = GenericMessages.danger
                    });
                    return RedirectToAction("List", "Payment");
                }

                // Only the payer (or an admin) may complete this payment.
                if (payment.User.Id != LoggedOnReadOnlyUser.Id && !User.IsInRole(AppConstants.AdminRoleName))
                {
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
                }

                // Idempotent: if this order was already confirmed, don't capture again.
                if (payment.PaymentStatus == PaymentStatus.Confirmed)
                {
                    ShowMessage(new GenericMessageViewModel
                    {
                        Message = LocalizationService.GetResourceString("PayPal.Success"),
                        MessageType = GenericMessages.success
                    });
                    return RedirectToAction("List", "Payment");
                }

                // Verify the money server-side before granting anything.
                var capture = payPal.CaptureOrder(token);
                if (!capture.Success)
                {
                    LoggingService.Error("PayPal capture failed for order " + token + ": " + capture.Error);
                    ShowMessage(new GenericMessageViewModel
                    {
                        Message = LocalizationService.GetResourceString("PayPal.Failed"),
                        MessageType = GenericMessages.danger
                    });
                    return RedirectToAction("List", "Payment");
                }

                // Activate the ad - mirrors the admin Confirm flow.
                var package = payment.Package;
                var advert = payment.Advertisement;
                advert.Package = payment.Package;
                if (payment.PaymentStatus == PaymentStatus.Pending)
                {
                    advert.DateStart = DateTime.UtcNow;
                }
                var adCloseAfter = (payment.Month * AppConstants.TimeSpanMonthDays) + package.Bonus;
                advert.DateEnd = DateTime.UtcNow.AddDays(adCloseAfter);
                advert.AdStatus = AdStatus.Active;
                payment.InvoiceNo = _paymentService.GetConfirmedPaymentsCount() + 1;
                payment.PaymentStatus = PaymentStatus.Confirmed;
                payment.ConfirmedBy = loggedOnUser.Id;
                payment.DateConfirmed = DateTime.Now;
                payment.Comment = string.Concat(payment.Comment, " | captured ", capture.CapturedAmount.ToString("0.00"), " ",
                    capture.CurrencyCode, " (", capture.CaptureId, ")");

                // Best-effort confirmation email (never blocks activation).
                try
                {
                    var user = MembershipService.GetUser(payment.User.Id);
                    var email = new Email
                    {
                        EmailTo = user.Email,
                        NameTo = user.UserName,
                        Subject = string.Concat(LocalizationService.GetResourceString("Payment Confirmed" + " "), settings.ForumName)
                    };
                    var sb = new StringBuilder();
                    sb.AppendFormat("<p>{2} : <a href=\"{0}\">{1}</a></p>",
                        string.Concat(settings.ForumUrl.TrimEnd('/'), advert.NiceUrl), advert.Name,
                        LocalizationService.GetResourceString("Payment Confirmed in"));
                    email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                    _emailService.SendMail(email);
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex);
                }

                try
                {
                    unitOfWork.Commit();
                    ShowMessage(new GenericMessageViewModel
                    {
                        Message = LocalizationService.GetResourceString("PayPal.Success"),
                        MessageType = GenericMessages.success
                    });
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    ShowMessage(new GenericMessageViewModel
                    {
                        Message = LocalizationService.GetResourceString("PayPal.Failed"),
                        MessageType = GenericMessages.danger
                    });
                }
            }
            return RedirectToAction("List", "Payment");
        }

        // Payer clicked "Cancel and return" on PayPal. Drop the abandoned Pending
        // payment so the ad reverts to Partial and can be paid again cleanly.
        [Authorize]
        public ActionResult PayPalCancel(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var payment = _paymentService.GetPaymentByReference(token).FirstOrDefault();
                    if (payment != null
                        && payment.PaymentStatus == PaymentStatus.Pending
                        && payment.User.Id == LoggedOnReadOnlyUser.Id)
                    {
                        var advert = payment.Advertisement;
                        _paymentService.Delete(payment);
                        if (advert != null && advert.AdStatus == AdStatus.Pending)
                        {
                            advert.AdStatus = AdStatus.Partial;
                        }
                        try
                        {
                            unitOfWork.Commit();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                        }
                    }
                }
            }
            ShowMessage(new GenericMessageViewModel
            {
                Message = LocalizationService.GetResourceString("PayPal.Cancelled"),
                MessageType = GenericMessages.info
            });
            return RedirectToAction("List", "Payment");
        }

        [HttpPost]
        [Authorize(Roles = AppConstants.AdminRoleName)]
        //[ValidateAntiForgeryToken]
        public void Confirm(PaymentConfirmationViewModel paymentConfirmationViewModel)
        {
            if (Request.IsAjaxRequest())
            {
                int adCloseAfter = 0;
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    //  if (User.Identity.IsAuthenticated && User.IsInRole(AppConstants.AdminRoleName))
                    // {
                    var settings = SettingsService.GetSettings();
                    var payment = _paymentService.Get(paymentConfirmationViewModel.Id);
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    //Get Package
                    var package = payment.Package;
                    //Get Advert
                    var advert = payment.Advertisement;
                    // Set Ad Package
                    advert.Package = payment.Package;
                    if (payment.PaymentStatus == PaymentStatus.Pending)
                    {
                        advert.DateStart = DateTime.UtcNow;
                    }

                    // Set Ad End date
                    adCloseAfter = (payment.Month * AppConstants.TimeSpanMonthDays) + package.Bonus;
                    advert.DateEnd = DateTime.UtcNow.AddDays(adCloseAfter);
                    advert.AdStatus = AdStatus.Active;
                    //Generate Invoice Number Get All Confirmed Payments Plus 1
                    payment.InvoiceNo = _paymentService.GetConfirmedPaymentsCount() + 1;
                    payment.PaymentStatus = PaymentStatus.Confirmed;
                    payment.ConfirmedBy = loggedOnUser.Id;
                    payment.DateConfirmed = DateTime.Now;
                    // Finally send payment confirmation email to the user
                    var user = MembershipService.GetUser(payment.User.Id);
                    var email = new Email
                    {
                        EmailTo = user.Email,
                        NameTo = user.UserName,
                        Subject = string.Concat(LocalizationService.GetResourceString("Payment Confirmed" + " "), settings.ForumName),
                    };

                    var sb = new StringBuilder();
                    sb.AppendFormat("<p>{2} <a href=\"{0}\">{1}</a></p>",
           /* string.Concat(settings.ForumUrl.TrimEnd('/'), loggedOnUser.NiceUrl), loggedOnUser.UserName, LocalizationService.GetResourceString("Invites"));*/
           //  Subject = LocalizationService.GetResourceString("PM.NewPrivateMessageSubject"),
           //  Message = "We Thank You" + user.UserName + "Your Payment is Verified,Your Ad Active Now",
           string.Concat(settings.ForumUrl.TrimEnd('/'), loggedOnUser.NiceUrl), loggedOnUser.UserName, "");
                    sb.AppendFormat("<p>{2} : <a href=\"{0}\">{1}</a></p>",
                   string.Concat(settings.ForumUrl.TrimEnd('/'), advert.NiceUrl), advert.Name, LocalizationService.GetResourceString("Payment Confirmed in"));

                    email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                    _emailService.SendMail(email);
                    //}
                    try
                    {
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
        }
        [HttpPost]
        [Authorize(Roles = AppConstants.AdminRoleName)]
        [ValidateAntiForgeryToken]
        public void Reject(PaymentRejectionViewModel rejected)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var payment = _paymentService.Get(rejected.Id);
                    var advert = _adService.Get(payment.Advertisement.Id);
                    advert.AdStatus = AdStatus.Partial;
                    //  var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    payment.PaymentStatus = PaymentStatus.Rejected;
                    unitOfWork.SaveChanges();
                    // if (User.Identity.IsAuthenticated && User.IsInRole(AppConstants.AdminRoleName))
                    //  {
                    try
                    {
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
        }
        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public void Delete(PaymentDeleteViewModel viewModel)
        {
            if (Request.IsAjaxRequest())
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var payment = _paymentService.Get(viewModel.Id);
                    var advert = _adService.Get(payment.Advertisement.Id);
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    if (payment.User.Id == LoggedOnReadOnlyUser.Id || User.IsInRole(AppConstants.AdminRoleName))
                    {
                        if (payment.PaymentStatus == PaymentStatus.Pending || payment.PaymentStatus == PaymentStatus.Rejected)
                        {
                            // Delete 
                            _paymentService.Delete(payment);
                            if (advert.AdStatus == AdStatus.Pending)
                            {
                                advert.AdStatus = AdStatus.Partial;
                            }
                        }
                        try
                        {
                            unitOfWork.Commit();
                            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Payment Deleted Successfully"),
                                MessageType = GenericMessages.success
                            };
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                        }
                    }
                }
            }
        }
        [Authorize]
        public ActionResult Edit(Guid paymentId)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                CreateEditPaymentViewModel viewModel = new CreateEditPaymentViewModel();
                var payment = _paymentService.Get(paymentId);
                var package = payment.Package;
                var partialKey = string.Concat("Package.", package.Name);
                package.DisplayName = LocalizationService.GetResourceString(string.Concat(partialKey, ".Name"));
                var grossPrice = payment.TotalPrice - payment.Discount;

                if (payment != null)
                {
                    viewModel.AdPackage = package;
                    viewModel.Advertisement = payment.Advertisement;
                    viewModel.User = payment.User;
                    viewModel.GrossPrice = (double)grossPrice;
                    viewModel.Discount = payment.Discount;
                    viewModel.TotalPrice = payment.TotalPrice;
                    viewModel.Amount = payment.Amount;
                    viewModel.Bank = payment.Bank;
                    viewModel.AccountNo = payment.AccountNo;
                    viewModel.DateTransfer = payment.DateTransfer;
                    viewModel.Month = payment.Month;
                    viewModel.Reference = payment.Reference;
                    viewModel.Comment = payment.Comment;
                    viewModel.Id = payment.Id;
                    viewModel.PackageId = payment.Package.Id;
                    viewModel.AdvertId = payment.Advertisement.Id;
                }
                return View(viewModel);
            }
        }
        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(CreateEditPaymentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var payment = _paymentService.Get(viewModel.Id);
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    // Edit Payment model
                    if (viewModel.Amount < payment.TotalPrice)
                    {
                        ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Payment.Error.InvalidAmount"));
                        return View(viewModel);
                    }
                    if (payment.Reference != viewModel.Reference)
                    {
                        var payments = _paymentService.GetPaymentByReference(viewModel.Reference);
                        if (payments.Any())
                        {
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Payment.Error.InvalidReference"));
                            return View(viewModel);
                        }
                    }
                    if (payment.User.Id == loggedOnUser.Id || User.IsInRole(AppConstants.AdminRoleName))
                    {
                        payment.Reference = viewModel.Reference;
                        payment.Amount = viewModel.Amount;
                        payment.DateTransfer = viewModel.DateTransfer;
                        payment.Bank = viewModel.Bank;
                        payment.AccountNo = viewModel.AccountNo;
                        payment.DateEdited = DateTime.UtcNow;
                        payment.Comment = viewModel.Comment;
                        unitOfWork.SaveChanges();
                        try
                        {
                            unitOfWork.Commit();
                            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Payment Successfully Update ,When Payment Confirmed Your Add Starts Automatically"),
                                MessageType = GenericMessages.info
                            };
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                            ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
                        }
                    }
                }
                if (User.IsInRole(AppConstants.AdminRoleName))
                {
                    return RedirectToAction("Index", "Payment");
                }
                return RedirectToAction("List", "Payment");
            }
            return View(viewModel);
        }
        [Authorize]
        public ActionResult Invoice(Guid paymentId)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var payment = _paymentService.Get(paymentId);
                var package = payment.Package;
                //Localize Package Display Name
                var partialKey = string.Concat("Package.", package.Name);
                package.DisplayName = LocalizationService.GetResourceString(string.Concat(partialKey, ".Name"));
                var user = payment.User;
                var advert = payment.Advertisement;
                if (payment.InvoiceNo <= 0)
                {
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = LocalizationService.GetResourceString("PM.AboutToExceedInboxSizeBody"),
                        MessageType = GenericMessages.danger
                    };
                    // Not allowed in here so
                    return RedirectToAction("List", "Payment");
                }
                InvoiceViewModel viewModel = new InvoiceViewModel
                {
                    Payment = payment,
                    Package = package,
                    Advertisement = advert,
                    User = user
                };
                return View(viewModel);
            }
        }
        [Authorize]
        public ActionResult Refund()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        // [ValidateAntiForgeryToken]
        public ActionResult Refund(RefundRequestViewModel viewModel)
        {
            var settings = SettingsService.GetSettings();
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    var payment = _paymentService.GetPaymentByReference(viewModel.Reference, LoggedOnReadOnlyUser.Id);
                    if (payment != null && payment.PaymentStatus == PaymentStatus.Confirmed)
                    {
                        var date = DateTime.UtcNow.AddDays(-AppConstants.RefundRequestPeriod);
                        if (payment.DateTransfer > date)
                        {
                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Payment.RefundRequest.PeriodEnd"),
                                MessageType = GenericMessages.danger
                            });
                            return RedirectToAction("List", "Payment");
                        }
                        if (payment.PaymentStatus == PaymentStatus.Refund)
                        {
                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Payment.RefundRequested"),
                                MessageType = GenericMessages.info
                            });
                            return RedirectToAction("List", "Payment");
                        }

                        payment.Comment = viewModel.Reason;
                        payment.PaymentStatus = PaymentStatus.Refund;
                        var advertisement = payment.Advertisement;
                        //  advertisement.AdStatus = AdStatus.Pending;

                        var sb = new StringBuilder();
                        sb.Append($"<p>{string.Format(_localizationService.GetResourceString("Help.Title.RefundRequest"), advertisement.Name, settings.ForumUrl)}</p>");
                        sb.Append(AppHelpers.ConvertPostContent(viewModel.Reason));
                        sb.AppendFormat("<p><a href=\"{0}\">{0}</a></p>", string.Concat(Domain, advertisement.NiceUrl));
                        sb.Append($"<p>{advertisement.User.UserName} - {advertisement.User.Email}</p>");
                        // create the email
                        var email = new Email
                        {
                            EmailTo = settings.AdminEmailAddress,
                            NameTo = _localizationService.GetResourceString("Members.Admin"),
                            Subject = string.Concat(LocalizationService.GetResourceString("Help.Title.RefundRequest"), SettingsService.GetSettings().ForumName)
                        };
                        // and now pass the email to be sent
                        email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString());
                        _emailService.SendMail(email);
                        try
                        {
                            unitOfWork.Commit();
                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Help.Label.RefundRequestSubmitted"),
                                MessageType = GenericMessages.success
                            });
                            return RedirectToAction("List", "Payment");
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            LoggingService.Error(ex);
                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = LocalizationService.GetResourceString("Help.Label.RefundRequestFailed"),
                                MessageType = GenericMessages.info
                            });
                        }
                        return RedirectToAction("List", "Payment");
                    }
                    else
                    {
                        return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
                    }
                }
            }
            return null;
        }
        public PartialViewResult Notification()
        {
            if (Request.IsAjaxRequest())
            {
                using (UnitOfWorkManager.NewUnitOfWork())
                {
                    if (UserIsAuthenticated)
                    {
                        //Get Member
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                        var memberAds = loggedOnUser.Advertisements.Where(x => x.AdStatus == AdStatus.Active).ToList();
                        var partialAds = loggedOnUser.Advertisements.Where(x => x.AdStatus == AdStatus.Partial && x.DateCreated < DateTime.UtcNow).ToList();
                        if (partialAds.Any())
                        {
                            var viewModel = new MemberNotificationViewModel
                            {
                                Subject = LocalizationService.GetResourceString("Payment.Notification.Subject"),
                                Notice = string.Format(LocalizationService.GetResourceString("Payment.Notification.Message"), loggedOnUser.UserName)
                            };
                            return PartialView(viewModel);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }
        [Authorize]
        public ActionResult Invoice1(Guid paymentId)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var payment = _paymentService.Get(paymentId);
                var package = payment.Package;
                var user = payment.User;
                var advert = payment.Advertisement;
                InvoiceViewModel viewModel = new InvoiceViewModel
                {
                    Payment = payment,
                    Package = package,
                    Advertisement = advert,
                    User = user
                };
                return View(viewModel);
            }
        }
    }
}
