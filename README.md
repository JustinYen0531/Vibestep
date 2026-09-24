# Vibestep

Vibestep 是一個 Windows 小工具。反白終端機指令後按下 `Ctrl + Shift + E`，它會在滑鼠右上方顯示一句繁體中文解釋與風險等級。

Vibestep 不會執行反白的指令。若文字疑似包含 API Key、密碼或存取權杖，內容不會送出。

## 第一次使用

1. 確認 Windows 使用者環境變數 `OPENAI_API_KEY` 已設定。
2. 在專案資料夾執行：

   ```powershell
   powershell -ExecutionPolicy Bypass -File .\build.ps1
   ```

3. 啟動：

   ```powershell
   .\artifacts\Vibestep.exe
   ```

4. 在終端機反白一段指令，按 `Ctrl + Shift + E`。
5. 按 `Esc` 或點擊其他地方關閉說明框。

程式啟動後會常駐在 Windows 右下角通知區。若要結束，對圖示按右鍵並選擇「離開 Vibestep」。

## 本機檢查

```powershell
powershell -ExecutionPolicy Bypass -File .\test.ps1
```

這會重新編譯程式並檢查本機風險判斷、敏感資料攔截與模型回應解析，不會送出 API 請求。

若要自行確認 API 連線，可執行：

```powershell
powershell -ExecutionPolicy Bypass -File .\live-test.ps1
```

這項測試會把無害的 `git status` 送到模型，並產生極小量 API 使用費。

## 重要限制

- 模型的說明只能協助閱讀，不能取代你的最終判斷。
- 部分軟體可能禁止程式取得反白文字。
- 若沒有反白內容，請不要使用快捷鍵；某些終端機可能把複製按鍵視為中止目前工作。
- API 使用量與費用由設定的 OpenAI API 帳戶負擔。

## 授權

[MIT](LICENSE) © 2026 Justin Yen
