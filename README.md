# TextTranslator
번역 프로그램  
사용 방법 :  
번역할 텍스트 파일 이름 + .translate 파일 생성  
그다음 파일에 이렇게 작성함 :  
using CodeBase:&&  
using ReadOrigin:=>  
using ReadConvert:==  
&&SetOriginLanguage:en  
&&TranslateStart  
  
  
  
CodeBase : 명령어 사용하기 전에 먼저 입력할 축약 문장  
ReadOrigin, ReadConvert : &&TranslateStart 이후에 사용할 명령어 축약  
&&SerOriginLanguage  
번역기 사용할때만 필요함, 번역할 언어 입력  
  
  
&&TranslateStart 이후 :  
  
일반 문장 입력 : 문장을 Text에 저장함  
ReadOrigin : Text에 저장된 문장을 OriginText에 저장  
ReadConvert : OriginText에 저장된 문장과 Text에 저장된 문장으로 변환식 생성  
&&Translator Google  
&&Translator Papago : OriginText에 저장된 문장을 번역기로 번역해 Text에 저장  
  
  
그외 명령어 :  
&&SetPapagoPKey:mypublickey  
&&SetPapagoSKey:mysecretkey  
파파고 번역 api 권한 변경  
  
  
  
예시 :  
원본 텍스트 파일 :  
asdfqwer  
i love you  
  
translator 파일 :  
using CodeBase:&&  
using ReadOrigin:=>  
using ReadConvert:==  
&&SetOriginLanguage:en  
&&TranslateStart  
sdfq  
=>  
1234  
==  
i love you  
=>
&&Translator Google  
==  
  
결과물 파일 :  
a1234wer  
사랑해  