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
