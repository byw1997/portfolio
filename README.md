# ペ・ヨンウ（Youngwoo Bae）ポートフォリオ

延世大学校（韓国）コンピュータ科学科、2028年3月卒業見込み。

大学の授業で書いたシステム系のコードをまとめています。授業課題はすべて個人で実装し、生成AIは使っていません。数値は当時の提出レポートに記録したものです。

## 一覧

| 分野 | 内容 | 言語 | 主な結果 |
| --- | --- | --- | --- |
| 並列 | [ロック付きハッシュテーブルの高速化](parallel/hash-table/) | C++ | 配布されたロック付きプロービング実装比 約17倍 |
| 並列 | [OpenMPによる行列積](parallel/matmul-openmp/) | C++ | 逐次の素朴な実装比 約472倍（1024×1024、32スレッド） |
| GPU | [CUDAリダクションの7段階の最適化](parallel/cuda-reduction/) | CUDA | 後半の最適化より起動構成の影響が大きかった |
| GPU | [CUDAによるVGG16推論](parallel/cuda-vgg16/) | CUDA | 6種のカーネルを実装、平均193 ms |
| OS | [スケジューラの追跡](os/kernel-schedule-trace/) | C | `__schedule()` を改変し直近20回の切り替えを /proc に出力 |
| OS | [ページテーブルの探索](os/kernel-page-walk/) | C | PGD→P4D→PUD→PMD→PTE をたどり物理アドレスを表示 |
| ネットワーク | [UDP上の信頼性のあるファイル転送](network/reliable-udp-p2p/) | Python | Selective Repeat（ウィンドウ100） |
| セキュリティ | [認証付き暗号化ツール](security/rust-authenticated-encryption/) | Rust | AES-256-CBC + HMAC-SHA256 |
| セキュリティ | [namespaceによるサンドボックス](security/rust-namespace-sandbox/) | Rust | mount・UTS・IPC・network namespace を分離 |
| その他 | [マルチ接続チャットサーバー](others/chat-server/) | C++ | 個人開発。WinSock + PostgreSQL |
| その他 | [データベース課題](others/db-assignment/) | Python, SQL | psycopg による検索・登録処理 |

## 授業

| 時期 | 授業 | このリポジトリの該当箇所 |
| --- | --- | --- |
| 2023年秋 | システムプログラミング | `os/` |
| 2023年秋 | コンピュータネットワーク | `network/` |
| 2023年秋 | データベース | `others/db-assignment/` |
| 2024年春 | Multi-core GPU Programming | `parallel/` |
| 2024年春 | 情報保護 | `security/` |

課題で配布された骨組みのコードやテスト用プログラムは含めていません。各ディレクトリの README に、自分で書いた範囲を記しています。
