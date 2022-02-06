import { Console } from 'console'
import Document, { Html, Head, Main, NextScript } from 'next/document'

import { tryGetGoogleAnalyticsId } from '../services/Configuration'

export default class MyDocument extends Document {

    getAnalyticsCode = () => {
        if(process.env.NODE_ENV === 'development') {
            return null
        }

        return <>
            {/* Global Site Tag (gtag.js) - Google Analytics */}
            <script
                async
                src={`https://www.googletagmanager.com/gtag/js?id=${tryGetGoogleAnalyticsId()}`}
                />
            <script
                dangerouslySetInnerHTML={{
                    __html: `
                window.dataLayer = window.dataLayer || [];
                function gtag(){dataLayer.push(arguments);}
                gtag('js', new Date());
                gtag('config', '${tryGetGoogleAnalyticsId()}');
                `,
                }}
                />
        </>
    }

    render() {
        return (
            <Html>
                <Head>
                    {
                        this.getAnalyticsCode()
                    }
                </Head>
                <body>
                    <Main />
                    <NextScript />
                </body>
            </Html>
        )
    }
}
