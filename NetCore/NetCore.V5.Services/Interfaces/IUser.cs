using NetCore.Data.Classes;
using NetCore.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.V5.Services.Interfaces
{
    public interface IUser
    {
        bool MatchTheUserInfo(LoginInfo login);

        User GetUserInfo(string userId);

        IEnumerable<UserRolesByUser> GetRolesOwnedByUser(string userId);

        /// <summary>
        /// [사용자 가입]
        /// </summary>
        /// <param name="register">사용자 가입용 뷰모델</param>
        /// <returns></returns>
        int RegisterUser(RegisterInfo register);

        /// <summary>
        /// [사용자 정보수정을 위한 검색]
        /// </summary>
        /// <param name="userId">사용자 아이디</param>
        /// <returns></returns>
        UserInfo GetUserInfoForUpdate(string userId);

        /// <summary>
        /// [사용자 정보수정]
        /// </summary>
        /// <param name="user">사용자정보 뷰모델</param>
        /// <returns></returns>
        int UpdateUser(UserInfo user);

        /// <summary>
        /// [사용자 정보수정에서 변경대상 비교] true : 전부 똑같을 때, false: 하나라도 다를 때
        /// </summary>
        /// <param name="user">사용자정보 뷰모델</param>
        /// <returns></returns>
        bool CompareInfo(UserInfo user);

        /// <summary>
        /// [사용자 탈퇴]
        /// </summary>
        /// <param name="user">사용자탈퇴정보 뷰모델</param>
        /// <returns></returns>
        int WithdrawnUser(WithdrawnInfo user);
    }
}
