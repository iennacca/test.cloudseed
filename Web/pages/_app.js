// import "tailwindcss/tailwind.css";
import '../styles/global.css';
import { AppContextWrapper } from '../state/AppContext'

// Required according to: https://nextjs.org/docs/basic-features/built-in-css-support
export default function MyApp({ Component, pageProps }) {
    return (
        <AppContextWrapper>
            <Component {...pageProps} />
        </AppContextWrapper>
    )
}