/*
Simple Rule Engine
Copyright (C) 2005 by Sierra Digital Solutions Corp

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace RuleEngine.Evidence
{
    /// <summary>
    ///  Evidence 结构体
    /// </summary>
    public struct EvidenceSpecifier
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="truthality">truthality</param>
        /// <param name="evidenceID">evidenceID</param>
        public EvidenceSpecifier(bool truthality, string evidenceID)
        {
            this.truthality = truthality;
            this.evidenceID = evidenceID;
        }
        /// <summary>
        /// truthality
        /// </summary>
        public bool truthality;

        /// <summary>
        /// evidenceID
        /// </summary>
        public string evidenceID;
    }
}
