﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveCard.PdfParser.Models;

namespace HiveCard.PdfParser.Interfaces
{
    public interface IExtractor
    {
        BankStatement Run(string pdfPath);
    }
}
