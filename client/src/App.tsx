import "./App.css";
import {createBrowserRouter, type RouteObject, RouterProvider} from "react-router-dom";

const myRoutes: RouteObject[]
= [
    {
        path: '/',
        element: <Home/>
    },
    {
        path: '/settings',
        element: <div>Hello there, this is settings route test:))))</div>
    }
]

function Home() {
    return (
        <>
            <div>home</div>
            <button onClick={() => {
                fetch(''/*baseUrl*/)
                    .then(response => {
                        console.log(response)
                    }).catch(e => {
                    console.log(e)
                })
            }}>click me!
            </button>
        </>
    )
}



function App() {
    
  return <RouterProvider router={createBrowserRouter(myRoutes)} />
}

export default App;
