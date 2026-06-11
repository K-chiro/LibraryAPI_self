using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Domains.Repositories;
using LibraryApi.Infrastructures.Adapters;
using LibraryApi.Infrastructures.Contexts;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Applications.Exceptions;

namespace LibraryApi.Infrastructures.Repositories;

public class BookCategoryRepository : IBookCategoryRepository
{

    //プロパティ
    public AppDbContext _context { get; set; }
    public BookCategoryEntityAdapter _adapter {get; set;}


    //コンストラクタ
    public BookCategoryRepository(AppDbContext context, BookCategoryEntityAdapter adapter)
    {
        _context = context ;
        _adapter = adapter ;
    }

    //カテゴリ一覧を取得
    public async Task<List<BookCategory>> SelectAllAsync()
    {
        try
        {
            var entities = await _context.Categories.AsNoTracking().ToListAsync();
            var categories = new List<BookCategory>();
            foreach (var entity in entities)
            {
                categories.Add(await _adapter.RestoreAsync(entity));
            }
            return categories;
        }
        catch (DomainException)
        {            
            throw;
        }
        catch(Exception ex)
        { 
            throw new InternalException("すべての商品カテゴリ取得時に予期しないエラーが発生しました。", ex);
        }
    }

    //カテゴリをID検索
    public async Task<BookCategory?> SelectByIdAsync(string id) 
    {
        try
        {
            var hit =await  _context.Categories.SingleOrDefaultAsync(c => c.CategoryUuid == id);
            if (hit is null)
            {
                return null;
            }
            var category = await _adapter.RestoreAsync(hit);
            return category;
        }
        catch (DomainException)
        {
            throw; // DomainException例外はそのまま再スローする
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする
            throw new InternalException($"Id:{id}の商品カテゴリ取得時に予期しないエラーが発生しました。", ex);
        }
    }
}
