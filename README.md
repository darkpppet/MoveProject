## MoveProject

랭크별로 폴더에 정리해놓은 백준 문제푼것들 중에 랭크 바뀐거 옮겨주는 프로그램

.NET 8.0

### src
ex)
```
src/
├─ BronzeV/
│    ├─ 1000.cs
│    ├─ 1001.cs
│    ...
...
```
### file naming
`digit[_word].word`

ex) `1000.cs`, `13705_Newton.cs`, `13277.py`... 

### solved.ac api 사용

* [api 문서 링크](https://solvedac.github.io/unofficial-documentation/#/)

* 그 중, [이 api](https://solvedac.github.io/unofficial-documentation/#/operations/getProblemsByIdList) 사용

### userinfo.json
```json
{
    "path": "./Baekjoon"
}
```
* `path`: 소스들이 있는 경로

### history
* 2022.10.25
  - 429 error 발생 시 1분 Sleep 후 다시 시도
  - 코드 리팩토링
  - 각 티어의 I단계 폴더가 생성되지 않는 오류 수정
  - 더 이상 사용되지 않는 WebClient를 HttpClient로 변경
  - 로그 파일 서식 수정

* 2024.09.15
  - 사용 api 변경
    - 더 이상 [이 api](https://solvedac.github.io/unofficial-documentation/#/operations/getProblemById)를 사용하지 않습니다
    - [이 api](https://solvedac.github.io/unofficial-documentation/#/operations/getProblemsByIdList)로 100문제씩 가져오게 됩니다
    - 따라서 429 error Sleep도 더 이상 하지 않습니다
    - 속도가 대폭 증가했습니다
  - RequestHeader를 추가하여 정상적으로 데이터를 가져옵니다

---

# Legacy

**(Not Use Anymore)**

.NET 6.0

## MoveProject1

백준 문제푼거들 랭크별로 폴더에 옮기려고 이동명령어 만드는 프로그램

### input
ex) `input.txt`
```txt
Bronze V 1000	A+B STANDARD	
167,054
2.35
Bronze III 2739	구구단	
101,238
1.95
...
```
solved.ac 페이지 가서 드래그하고 복사하면 이렇게 나옴

### output
ex) `order.sh`
```bash
#!/bin/bash
mv 1000.cs 1001.cs ... src/BronzeV
...
mv 2739.cs ... src/BronzeIII
...
```

### pathinfo.json
```json
{
    "inputPath": "input/input.txt",
    "outputPath": "output",
    "sourcePath": "src"
}
```
* `inputPath`: input.txt의 경로
* `outputPath`: 명령어들을 내보낼 경로
* `sourcePath`: 소스들을 옮길 경로(명령어에 사용)

<br>

---

## MoveProject2

**Not Use Anymore**

원드라이브에 있던 백준 문제푼것들 rename + 티어별로 폴더에 분류하는 프로그램

### OnDrive
ex)
```
src/
├─ 1000/
│    ├─ .vs/
│    ├─ 1000/
│    │    ├─ bin/
│    │    ├─ obj/
│    │    ├─ 1000.csproj
│    │    └─ Program.cs
│    └─ 1000.sln
├─ 1001/
...
```

### input
ex) `input.txt`
```txt
Bronze V 1000	A+B STANDARD	
167,054
2.35
Bronze III 2739	구구단	
101,238
1.95
...
```
solved.ac 페이지 가서 드래그하고 복사하면 이렇게 나옴

### output
ex)
```
output/
├─ BronzeV/
│    ├─ 1000.cs
│    ├─ 1001.cs
│    ...
...
```

### pathinfo.json
```json
{
    "inputPath": "input/input.txt",
    "outputPath": "output",
    "sourcePath": "../src"
}
```
* `inputPath`: input.txt의 경로
* `outputPath`: rename하고 분류한 소스들을 내보낼 경로
* `sourcePath`: onedrive 소스들이 있는 경로
