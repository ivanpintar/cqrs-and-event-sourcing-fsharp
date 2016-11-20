import React from 'react';
import SelectableProductList from '../../products/containers/SelectableProductList';
import BasketContainer from '../../baskets/containers/BasketContainer';

const App = () => (
    <div>
        <h1>Products</h1>
        <SelectableProductList/>
        <hr/>
        <h1>Basket</h1>
        <BasketContainer/>
    </div>
);

export default App;