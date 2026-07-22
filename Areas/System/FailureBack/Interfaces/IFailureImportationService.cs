using FiyiPOS.Areas.System.FailureBack.Entities;

/*
 * 
 * Coded by fiyistack.com
 * Copyright © 2026
 * 
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 * 
 */

namespace FiyiPOS.Areas.System.FailureBack.Interfaces
{
    public interface IFailureImportationService
    {
        List<Failure> ImportExcel(string path, long userId);
    }
}