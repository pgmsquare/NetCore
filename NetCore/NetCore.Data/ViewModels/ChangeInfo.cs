using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NetCore.Data.ViewModels
{
    public class ChangeInfo
    {
        /// <summary>
        /// 사용자 이름
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 사용자 이메일
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        /// <summary>
        /// true : 전부 똑같을 때, false: 하나라도 다를 때
        /// </summary>
        /// <param name="other">비교할 다른 클래스</param>
        /// <returns></returns>
        public bool Equals(UserInfo other)
        {
            if (!string.Equals(UserName, other.UserName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(UserEmail, other.UserEmail, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}
