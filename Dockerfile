FROM microsoft/dotnet

RUN mkdir -p /root/code/
ADD . /root/code/

CMD bash
