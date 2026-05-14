$path = "d:\Afroz\Expense Tracker (MVC Core)\ExpenseTracker\ExpenseTracker\Views"
$files = Get-ChildItem -Path $path -Filter *.cshtml -Recurse

foreach ($file in $files) {
    if ($file.Name -match "GetFullReport") { continue }

    $content = Get-Content $file.FullName -Raw

    $newContent = $content -replace 'background:\s*white;?', 'background: var(--card);'
    $newContent = $newContent -replace 'background-color:\s*white;?', 'background-color: var(--card);'
    $newContent = $newContent -replace 'background:\s*#f8fafc;?', 'background: var(--bg);'
    $newContent = $newContent -replace 'background-color:\s*#f8fafc;?', 'background-color: var(--bg);'
    $newContent = $newContent -replace 'background:\s*#f1f5f9;?', 'background: var(--border);'
    $newContent = $newContent -replace 'color:\s*#1e293b;?', 'color: var(--text-main);'
    $newContent = $newContent -replace 'color:\s*#0f172a;?', 'color: var(--text-main);'
    $newContent = $newContent -replace 'color:\s*#475569;?', 'color: var(--muted);'
    $newContent = $newContent -replace 'color:\s*#64748b;?', 'color: var(--muted);'
    $newContent = $newContent -replace 'border:\s*1px solid #f1f5f9;?', 'border: 1px solid var(--border);'
    $newContent = $newContent -replace 'border:\s*1px solid rgba\(0,0,0,0\.02\);?', 'border: 1px solid var(--border);'
    $newContent = $newContent -replace 'border:\s*1px solid #e2e8f0;?', 'border: 1px solid var(--border);'
    $newContent = $newContent -replace 'border:\s*1\.5px solid #e2e8f0;?', 'border: 1.5px solid var(--border);'
    $newContent = $newContent -replace 'border-bottom:\s*1px solid #f1f5f9;?', 'border-bottom: 1px solid var(--border);'
    $newContent = $newContent -replace '\btext-dark\b', 'text-main-adaptive'
    $newContent = $newContent -replace 'background:\s*#fef2f2;?', 'background: var(--card);'
    $newContent = $newContent -replace 'background:\s*#ecfdf5;?', 'background: var(--card);'

    if ($content -ne $newContent) {
        Set-Content -Path $file.FullName -Value $newContent
    }
}
