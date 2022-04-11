using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NetCore.Data.ViewModels;
using NetCore.V5.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore.Web.V6.Controllers
{
    [Authorize(Roles = "AssociateUser, GeneralUser, SuperUser, SystemUser")]
    public class MembershipController : Controller
    {
        //닷넷코어는 의존성 주입 패턴을 가지는데요.
        //생성자를 통해서 의존성 주입이 이루어집니다.
        private IUser _user;
        private IPasswordHasher _hasher;
        private HttpContext _context;

        public MembershipController(IHttpContextAccessor accessor, IPasswordHasher hasher, IUser user)
        {
            _context = accessor.HttpContext;
            _hasher = hasher;
            _user = user;
        }

        #region private methods
        /// <summary>
        /// 로컬URL인지 외부URL인지 체크
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(MembershipController.Index), "Membership");
            }
        }
        #endregion

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View(new LoginInfo());
        }

        //[HttpPost("/Login")]
        // Action만 지정하던 것에서 Controller까지 같이 지정하는 것으로
        // .Net Core 3.1에서 변경됨.
        [HttpPost("/{controller}/Login")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        //Data => Services => Web
        //Data => Services
        //Data => Web
        public async Task<IActionResult> LoginAsync(LoginInfo login, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //뷰모델을 사용.
                //데이터베이스를 통해서 데이터모델이 연동되어 값을 가지고 있어서
                //그것과 입력받은 값들과 비교를 해야됩니다.
                //그런 서비스가 필요.
                //서비스개념을 가져와야..
                if (_user.MatchTheUserInfo(login))
                //if (_hasher.MatchTheUserInfo(login.UserId, login.Password))
                {
                    //신원보증과 승인권한
                    var userInfo = _user.GetUserInfo(login.UserId);
                    var roles = _user.GetRolesOwnedByUser(login.UserId);
                    var userTopRole = roles.FirstOrDefault();
                    string userDataInfo = userTopRole.UserRole.RoleName + "|" +
                                          userTopRole.UserRole.RolePriority.ToString() + "|" +
                                          userInfo.UserName + "|" +
                                          userInfo.UserEmail;

                    //_context.User.Identity.Name => 사용자 아이디

                    var identity = new ClaimsIdentity(claims: new[]
                    {
                    new Claim(type:ClaimTypes.Name,
                              value:userInfo.UserId),
                    new Claim(type:ClaimTypes.Role,
                              value:userTopRole.RoleId),
                    new Claim(type:ClaimTypes.UserData,
                              value:userDataInfo)
                    }, authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);

                    await _context.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                                               principal: new ClaimsPrincipal(identity: identity),
                                               properties: new AuthenticationProperties()
                                               {
                                                   IsPersistent = login.RememberMe,
                                                   ExpiresUtc = login.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30)
                                               });

                    TempData["Message"] = "로그인이 성공적으로 이루어졌습니다.";

                    //return RedirectToAction("Index", "Membership");
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    message = "로그인되지 않았습니다.";
                }
            }
            else
            {
                message = "로그인정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View("Login", login);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View(new RegisterInfo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Register(RegisterInfo register, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //사용자 가입 서비스
                if (_user.RegisterUser(register) > 0)
                {
                    TempData["Message"] = "사용자 가입이 성공적으로 이루어졌습니다.";
                    return RedirectToAction("Login", "Membership");
                }
                else
                {
                    message = "사용자가 가입되지 않았습니다.";
                }
            }
            else
            {
                message = "사용자 가입을 위한 정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(register);
        }

        [HttpGet]
        public IActionResult UpdateInfo()
        {
            UserInfo user = _user.GetUserInfoForUpdate(_context.User.Identity.Name);//서비스 

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInfo(UserInfo user)
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //변경대상 값들을 비교 서비스
                if (_user.CompareInfo(user))
                {
                    message = "하나 이상의 값이 변경되어야 정보수정이 가능합니다.";
                    ModelState.AddModelError(string.Empty, message);
                    return View(user);
                }

                //정보수정 서비스
                if (_user.UpdateUser(user) > 0)
                {
                    TempData["Message"] = "사용자 정보수정이 성공적으로 이루어졌습니다.";

                    return RedirectToAction("UpdateInfo", "Membership");
                }
                else
                {
                    message = "사용자 정보가 수정되지 않았습니다.";
                }
            }
            else
            {
                message = "사용자 정보수정을 위한 정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(user);
        }

        //[HttpPost("/Withdrawn")]
        // Action만 지정하던 것에서 Controller까지 같이 지정하는 것으로
        // .Net Core 3.1에서 변경됨.
        [HttpPost("/{controller}/Withdrawn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WithdrawnAsync(WithdrawnInfo withdrawn)
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //탈퇴 서비스
                if (_user.WithdrawnUser(withdrawn) > 0)
                {
                    TempData["Message"] = "사용자 탈퇴가 성공적으로 이루어졌습니다.";

                    await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);

                    return RedirectToAction("Index", "Membership");
                }
                else
                {
                    message = "사용자가 탈퇴처리되지 않았습니다.";
                }

            }
            else
            {
                message = "사용자가 탈퇴하기 위한 정보를 올바르게 입력하세요.";
            }

            ViewData["Message"] = message;
            return View("Index", withdrawn);
        }

        //[HttpGet("/LogOut")]
        // Action만 지정하던 것에서 Controller까지 같이 지정하는 것으로
        // .Net Core 3.1에서 변경됨.
        [HttpGet("/{controller}/LogOut")]
        public async Task<IActionResult> LogOutAsync()
        {
            await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Message"] = "로그아웃이 성공적으로 이루어졌습니다. <br />웹사이트를 원활히 이용하시려면 로그인하세요.";

            return RedirectToAction("Index", "Membership");
        }

        [HttpGet]
        //[Authorize(Roles = "AssociateUser")]
        public IActionResult Forbidden([FromServices] ILogger<MembershipController> logger)
        {
            //paramReturnUrl[0] => /Data/AES
            bool exists = _context.Request.Query.TryGetValue("returnUrl", out StringValues paramReturnUrl);
            paramReturnUrl = exists ? _context.Request.Host.Value + paramReturnUrl[0] : string.Empty;

            logger.LogInformation($"{MethodBase.GetCurrentMethod().Name} 메서드.권한이 없는 사람이 페이지에 접근 에러 처리.returnUrl : {paramReturnUrl}");

            ViewData["Message"] = $"귀하는 {paramReturnUrl} 경로로 접근하려고 했습니다만,<br />" +
                                   "인증된 사용자도 접근하지 못하는 페이지가 있습니다.<br />" +
                                   "담당자에게 해당페이지의 접근권한에 대해 문의하세요.";

            return View();
        }
    }
}
